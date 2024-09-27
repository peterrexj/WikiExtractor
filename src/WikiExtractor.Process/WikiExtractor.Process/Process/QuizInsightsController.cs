using Pj.Library;
using WikiExtractor.Process.Exts;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;

namespace WikiExtractor.Process.Process
{
    public class QuizInsightsController(IWikiDatabase wikiDb, IUserStoreDatabase userStoreDb)
    {
        public void ExportQuizDataVisualQuestionsToCsv(string appName)
        {
            var questionsCollection = from masterMeta in wikiDb.QuizMasterMetadataRepository.GetAll()
                                          //where masterMeta.MasterId == 1
                                      join master in wikiDb.MasterRepository.GetAll()
                                          on masterMeta.MasterId equals master.Id into masterGroup
                                      from masterEntry in masterGroup.DefaultIfEmpty() // handle no match in masterGroup

                                      join metadata in wikiDb.MetadataRepository.GetAll()
                                          on new { masterMeta.MasterId, MetadataKey = masterMeta.MetadataKey.ToLower() }
                                          equals new { metadata.MasterId, MetadataKey = metadata.Key.ToLower() } into metadataGroup
                                      from metadataEntry in metadataGroup.DefaultIfEmpty() // handle no match in metadataGroup

                                      join questionDef in wikiDb.QuizDefinitionRepository.GetAll()
                                          on masterMeta.MetadataKey equals questionDef.MetadataKey into questionDefGroup
                                      from questionDefEntry in questionDefGroup.DefaultIfEmpty() // handle no match in questionDefGroup

                                      select new
                                      {
                                          Name = masterEntry != null ? masterEntry.Name : "N/A", // handle null
                                          Item = masterMeta.MetadataKey,
                                          Question = (questionDefEntry?.QuestionPhrase ?? "")
                                              .Replace("{MasterId}", masterEntry != null ? masterEntry.Name : "N/A"),
                                          Answer = metadataEntry.Value,
                                          AnswerTextLength = metadataEntry.Value.Length
                                      };

            CsvHelperEx.WriteToCsv(questionsCollection, IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Insights", $"{appName}_QuizData_Visualize_Questions_{DateTimeEx.GetDateTimeReadable()}.csv"), hasHeaderRecords: true);


            var textData = new List<(string Text, int Length)>();

            foreach (var item in questionsCollection)
            {
                if (!string.IsNullOrEmpty(item.Answer))
                {
                    textData.Add((item.Answer, item.Answer.Length));
                }
            }

            var stats = new TextStatistics(textData);
            var result = stats.GetLengthStatistics(40); // 5 buckets

            CsvHelperEx.WriteToCsv(result.Select(f => new { f.Range, f.Count, f.Percentage}).ToList(), IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Insights", $"{appName}_QuizData_Visualize_StatisticsData_{DateTimeEx.GetDateTimeReadable()}.csv"), hasHeaderRecords: true);
        }

        public void QuizDataInsightsToBuildQuiz(string appName)
        {
            var metadataList = wikiDb.MetadataRepository.Get(f => f.TypeByEnum.ToString() == "Detail").Where(f => f.Value.HasValue()).ToList();
            var properties = metadataList.GroupBy(f => f.Key)
                .Select(f => new
                {
                    Filter = f.Key,
                    Values = f.Select(c => c.Value).ToList()
                })
                .OrderByDescending(f => f.Values.Count)
                .ToList();

            var filePath = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Insights",
                $"{appName}_MetadataGroups_{DateTimeEx.GetDateTimeReadable()}.csv");

            IoHelper.CreateDirectory(filePath);

            CsvHelperEx.WriteToCsv(properties.Select(p => new { Metadata = p.Filter, p.Values.Count }).ToList(), filePath, hasHeaderRecords: true);

            var insights =
                from metadata in wikiDb.MetadataRepository.Get(f => f.TypeByEnum.ToString() == "Detail").ToList()

                join master in wikiDb.MasterRepository.GetAll()
                    on metadata.MasterId equals master.Id into masterGroup
                from masterEntry in masterGroup.DefaultIfEmpty() // handle no match in masterGroup

                join property in properties
                    on metadata.Key equals property.Filter into propertyGroup
                from propertyEntry in propertyGroup.DefaultIfEmpty()

                where metadata.Value.HasValue()

                select new
                {
                    masterEntry.Name,
                    Count = propertyEntry?.Values?.Count ?? 0,
                    Metadata = metadata.Key,
                    metadata.Value,
                };

            var filePath2 = IoHelper.CombinePath(PjUtility.Runtime.ExecutingFolder, "Insights",
                $"{appName}_MetadataValue_{DateTimeEx.GetDateTimeReadable()}.csv");

            IoHelper.CreateDirectory(filePath2);
            CsvHelperEx.WriteToCsv(insights.OrderByDescending(f => f.Count).ThenBy(f => f.Metadata).ToList(), filePath2, hasHeaderRecords: true);
        }
    }
}
