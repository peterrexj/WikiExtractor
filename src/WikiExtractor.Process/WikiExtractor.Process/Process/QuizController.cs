using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Pj.Library;
using WikiApp;
using WikiExtractor.DbModels;
using WikiExtractor.Process.DbModels;
using WikiExtractor.Process.Models;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;
using WikiExtractor.ViewModels;

namespace WikiExtractor.Process.Process
{
    public class QuizController
    {
        private readonly IWikiDatabase _wikiDb;
        private readonly IUserStoreDatabase _userStoreDb;

        public QuizController(IWikiDatabase wikiDb, IUserStoreDatabase userStoreDb)
        {
            _wikiDb = wikiDb;
            _userStoreDb = userStoreDb;
        }

        public List<QuizQuestionViewModel> GenerateQuizQuestionsForNewSession()
        {
            // Retrieve data from the database once and work with it in-memory
            var quizDefinitionsDb = _wikiDb.QuizDefinitionRepository.GetAll().ToList();
            var quizResponseDb = _userStoreDb.QuizResponseRepository.GetAll().ToList();
            var quizMasterMetadataDb = _wikiDb.QuizMasterMetadataRepository.GetAll().ToList();

            // Use a HashSet for faster lookups
            var quizResponseKeys = new HashSet<(int MasterId, string MetadataKey)>(
                quizResponseDb.Select(qr => (qr.MasterId, qr.MetadataKey))
            );

            var newQuizOptions = quizMasterMetadataDb
                .Where(qmm => !quizResponseKeys.Contains((qmm.MasterId, qmm.MetadataKey)))
                .ToList();

            if (!newQuizOptions.Any())
            {
                // take action to reset and start the quiz again
            }

            int questionCount = 10; //TODO: Change to 10 or configurable
            var randomQuestions = RandomGeneratorHelper.RandomizeSubset(newQuizOptions, questionCount, ensureUnique: false);

            var masterIds = new HashSet<int>(randomQuestions.Select(q => q.MasterId));
            var metadataKeys = new HashSet<string>(randomQuestions.Select(q => q.MetadataKey).Distinct(), StringComparer.OrdinalIgnoreCase);

            var masterDataInvolvedInQuizWithPictures = (from master in _wikiDb.MasterRepository.GetAll()
                                                        where masterIds.Contains(master.Id)
                                                        join pic in _wikiDb.WikiPictureRepository.GetAll()
                                                            on master.Id equals pic.MasterId into picsGroup
                                                        from pic in picsGroup.DefaultIfEmpty(new WikiPicture
                                                        {
                                                            MasterId = master.Id,
                                                            Path = "NoImageAvailable.png",
                                                            Caption = string.Empty
                                                        })
                                                        select new
                                                        {
                                                            Master = master,
                                                            PrimaryPicture = picsGroup.FirstOrDefault(f => f.Path.HasValue() && f.IsPrimaryBool)
                                                        }).ToList();

            var metadataRawDb = _wikiDb.MetadataRepository.GetAll()
                .Where(f => metadataKeys.Contains(f.Key))
                .ToList();

            var masterMetadataGroups = _wikiDb.MetadataRepository.GetAll()
                .Where(m => masterIds.Contains(m.MasterId))
                .GroupBy(m => m.MasterId)
                .ToDictionary(g => g.Key, g => g.ToList());

            List<QuizQuestionViewModel> listOfQuestionBuildUp = new();
            int index = 1;
            foreach (var randomQuestion in randomQuestions)
            {
                var masterData = masterDataInvolvedInQuizWithPictures.First(f => f.Master.Id == randomQuestion.MasterId);
                var questionText = quizDefinitionsDb
                    .FirstOrDefault(f => f.MetadataKey.EqualsIgnoreCase(randomQuestion.MetadataKey))
                    ?.QuestionPhrase.Replace("{MasterId}", masterData.Master.Name);

                var correctMetadata = masterMetadataGroups[randomQuestion.MasterId]
                    .FirstOrDefault(f => f.Key.EqualsIgnoreCase(randomQuestion.MetadataKey));

                if (correctMetadata == null)
                    continue;

                var correctAnswer = correctMetadata.Value;

                if (!IsMeaningfulAnswer(correctAnswer))
                    continue;

                var randomAnswers = RandomGeneratorHelper.RandomizeSubset(
                    metadataRawDb
                        .Where(f => f.Key.EqualsIgnoreCase(randomQuestion.MetadataKey) && f.Value != correctAnswer && IsMeaningfulAnswer(f.Value))
                        .Select(f => f.Value)
                        .Distinct()
                        .ToList(), 3, ensureUnique: true)
                    .ToList();

                randomAnswers.Add(correctAnswer);

                listOfQuestionBuildUp.Add(new QuizQuestionViewModel
                {
                    MasterId = randomQuestion.MasterId,
                    MasterName = masterData.Master.Name,
                    MasterPicPath = masterData.PrimaryPicture?.Path ?? "NoImageAvailable.png",
                    MasterPicHeight = masterData.PrimaryPicture?.Height ?? 0,
                    MasterPicWidth = masterData.PrimaryPicture?.Width ?? 0,
                    MetadataKey = randomQuestion.MetadataKey,
                    Question = questionText,
                    CorrectAnswer = correctAnswer,
                    AnswerCollection = new ObservableCollection<string>(RandomGeneratorHelper.RandomizeList(randomAnswers)),
                    Index = index++
                });
            }

            return listOfQuestionBuildUp;
        }

        public void QuizEnableDbWithDetails(List<QuizDefinitionJsonModel> quizDefinitionData)
        {
            //Used by the builder/extractor
            if (quizDefinitionData.Count == 0)
            {
                throw new Exception("There is no data in the argument passed!");
            }

            var metadatas = quizDefinitionData.Where(f => f.Metadata.HasValue()).Select(f => f.Metadata).ToList();
            var metadataDb = (from f in _wikiDb.MetadataRepository.GetAll()
                              join quiz in quizDefinitionData
                                  on f.Key.ToLower() equals quiz.Metadata.ToLower()
                              where quiz.Metadata.HasValue()
                              group new { f, quiz } by f.Key into grouped
                              select new
                              {
                                  Key = grouped.Key,
                                  Childs = grouped.Where(g =>
                                          IsMeaningfulAnswer(g.f.Value) &&
                                          (g.quiz.MaxLengthForAnswer == 0 ||
                                          g.f.Value.Length <= g.quiz.MaxLengthForAnswer))
                                      .Select(g => g.f)
                                      .ToList()
                              }).ToList();

            foreach (var metadata in metadatas)
            {
                var dataFromDb = metadataDb.FirstOrDefault(f => f.Key.EqualsIgnoreCase(metadata));
                var dataDefinition = quizDefinitionData.FirstOrDefault(f => f.Metadata.EqualsIgnoreCase(metadata));
                if (dataFromDb?.Childs.Count() > 20)
                {
                    _wikiDb.QuizDefinitionRepository.Add(new QuizDefinition
                    { MetadataKey = metadata, QuestionPhrase = dataDefinition?.QuestionRephrase, Fact = dataDefinition?.Fact }, checkAlreadyExists: true);
                }

                foreach (var metadataChild in dataFromDb.Childs)
                {
                    Console.WriteLine($"Inserting Master - Metadata information between {metadataChild.MasterId} and {metadataChild.Key}");
                    _wikiDb.QuizMasterMetadataRepository.Add(new QuizMasterMetadata
                    {
                        MasterId = metadataChild.MasterId,
                        MetadataKey = metadataChild.Key
                    }, checkAlreadyExists: true);
                }
            }

            var dataDefinitionsDb = _wikiDb.QuizDefinitionRepository.GetAll();
            if (!dataDefinitionsDb.Any())
            {
                throw new Exception("The definition data was generated, check the code");
            }

            var quizMasterMetadataDb = _wikiDb.QuizMasterMetadataRepository.GetAll();
            if (!quizMasterMetadataDb.Any())
            {
                throw new Exception("The master metadata data was generated, check the code");
            }

        }

        public int GetQuestionSetId()
        {
            return _userStoreDb.QuizResponseRepository.GetNewQuestionSetId();
        }

        public void SaveResponse(QuizResponse responseModel)
        {
            _userStoreDb.QuizResponseRepository.Add(responseModel, checkAlreadyExists: false);
        }

        public QuizStatsModel GetQuizStats()
        {
            var all = _userStoreDb.QuizResponseRepository.GetAll().ToList();

            var totalCorrect  = all.Count(r => r.UserResponse == 1);
            var totalWrong    = all.Count(r => r.UserResponse == 0);
            var totalSkipped  = all.Count(r => r.UserResponse == -1);
            var totalSessions = all.Select(r => r.QuestionSetId).Distinct().Count();

            // Per-session scores (last 15 sessions)
            var sessionScores = all
                .GroupBy(r => r.QuestionSetId)
                .OrderBy(g => g.Key)
                .TakeLast(15)
                .Select(g => new QuizSessionScore
                {
                    SessionId   = g.Key,
                    Correct     = g.Count(r => r.UserResponse == 1),
                    Total       = g.Count(),
                    PlayedAt    = g.Min(r => r.CreatedDateTime)
                })
                .ToList();

            // Per-topic accuracy (MetadataKey)
            var topicAccuracy = all
                .GroupBy(r => r.MetadataKey)
                .Where(g => g.Count(r => r.UserResponse != -1) >= 3)
                .Select(g => new QuizTopicAccuracy
                {
                    Topic    = g.Key,
                    Correct  = g.Count(r => r.UserResponse == 1),
                    Answered = g.Count(r => r.UserResponse != -1)
                })
                .OrderBy(t => t.Correct / (double)Math.Max(t.Answered, 1))
                .ToList();

            // Hardest subjects (bottom 5 by accuracy, min 2 answered)
            var subjectAccuracy = all
                .GroupBy(r => r.MasterId)
                .Where(g => g.Count(r => r.UserResponse != -1) >= 2)
                .Select(g => new QuizSubjectAccuracy
                {
                    MasterId = g.Key,
                    Correct  = g.Count(r => r.UserResponse == 1),
                    Answered = g.Count(r => r.UserResponse != -1)
                })
                .OrderBy(s => s.Correct / (double)Math.Max(s.Answered, 1))
                .Take(5)
                .ToList();

            // Resolve master names
            var masterIds  = subjectAccuracy.Select(s => s.MasterId).ToList();
            var masterNames = _wikiDb.MasterRepository.GetAll()
                .Where(m => masterIds.Contains(m.Id))
                .ToDictionary(m => m.Id, m => m.Name);
            foreach (var s in subjectAccuracy)
                s.MasterName = masterNames.TryGetValue(s.MasterId, out var n) ? n : s.MasterId.ToString();

            return new QuizStatsModel
            {
                TotalCorrect   = totalCorrect,
                TotalWrong     = totalWrong,
                TotalSkipped   = totalSkipped,
                TotalSessions  = totalSessions,
                SessionScores  = sessionScores,
                TopicAccuracy  = topicAccuracy,
                SubjectAccuracy = subjectAccuracy
            };
        }

        /// <summary>
        /// Fetches quiz facts that haven't been shown to the user yet.
        /// Facts are fetched from QuizDefinition and have MasterId and AnswerId placeholders replaced.
        /// </summary>
        /// <param name="count">Number of facts to fetch</param>
        /// <param name="masterId">Optional filter to get facts only for a specific master. If null, fetches from all masters.</param>
        /// <returns>List of formatted quiz facts ready to display</returns>
        public List<QuizFactViewModel> GetQuizFacts(int count, int? masterId = null)
        {
            if (count <= 0)
            {
                return new List<QuizFactViewModel>();
            }

            try
            {
                // Get all quiz definitions that have facts
                var quizDefinitionsWithFacts = _wikiDb.QuizDefinitionRepository.GetAll()
                    .Where(qd => !string.IsNullOrWhiteSpace(qd.Fact))
                    .ToList();

                if (!quizDefinitionsWithFacts.Any())
                {
                    return new List<QuizFactViewModel>();
                }

                // Get facts that have already been shown to the user
                var shownFacts = _userStoreDb.QuizFactStatusRepository.GetAll()
                    .Select(qfs => new { qfs.MasterId, qfs.MetadataKey })
                    .ToHashSet();

                // Get all quiz master metadata (combinations of master and metadata)
                var quizMasterMetadata = _wikiDb.QuizMasterMetadataRepository.GetAll();

                // Filter by masterId if provided
                if (masterId.HasValue)
                {
                    quizMasterMetadata = quizMasterMetadata.Where(qmm => qmm.MasterId == masterId.Value);
                }

                var quizMasterMetadataList = quizMasterMetadata.ToList();

                // Filter out facts that have already been shown
                var availableFacts = quizMasterMetadataList
                    .Where(qmm => !shownFacts.Contains(new { qmm.MasterId, qmm.MetadataKey }))
                    .ToList();

                // If no available facts, optionally we could reset and show all again
                if (!availableFacts.Any())
                {
                    availableFacts = quizMasterMetadataList;
                }

                // Randomly select the requested number of facts
                var selectedFacts = RandomGeneratorHelper.RandomizeSubset(availableFacts, 
                    Math.Min(count, availableFacts.Count), 
                    ensureUnique: true);

                // Get master IDs and metadata keys for efficient lookup
                var masterIds = selectedFacts.Select(sf => sf.MasterId).Distinct().ToList();
                var metadataKeys = selectedFacts.Select(sf => sf.MetadataKey).Distinct().ToList();

                // Fetch masters with their primary pictures
                var mastersWithPictures = (from master in _wikiDb.MasterRepository.GetAll()
                                          where masterIds.Contains(master.Id)
                                          join pic in _wikiDb.WikiPictureRepository.GetAll()
                                              on master.Id equals pic.MasterId into picsGroup
                                          from pic in picsGroup.DefaultIfEmpty(new WikiPicture
                                          {
                                              MasterId = master.Id,
                                              Path = "NoImageAvailable.png"
                                          })
                                          select new
                                          {
                                              Master = master,
                                              PrimaryPicture = picsGroup.FirstOrDefault(p => p.Path.HasValue() && p.IsPrimaryBool)
                                          }).ToList();

                // Fetch metadata values
                var metadataValues = _wikiDb.MetadataRepository.GetAll()
                    .Where(m => masterIds.Contains(m.MasterId) && metadataKeys.Contains(m.Key))
                    .ToList();

                // Build the result
                var result = new List<QuizFactViewModel>();

                foreach (var selectedFact in selectedFacts)
                {
                    var masterData = mastersWithPictures.FirstOrDefault(m => m.Master.Id == selectedFact.MasterId);
                    if (masterData == null) continue;

                    var quizDefinition = quizDefinitionsWithFacts.FirstOrDefault(qd => 
                        qd.MetadataKey.Equals(selectedFact.MetadataKey, StringComparison.OrdinalIgnoreCase));
                    if (quizDefinition == null) continue;

                    var metadataValue = metadataValues.FirstOrDefault(m => 
                        m.MasterId == selectedFact.MasterId && 
                        m.Key.Equals(selectedFact.MetadataKey, StringComparison.OrdinalIgnoreCase));
                    if (metadataValue == null) continue;

                    // Replace placeholders in the fact
                    var factText = quizDefinition.Fact
                        .Replace("{MasterId}", masterData.Master.Name)
                        .Replace("{AnswerId}", metadataValue.Value);

                    result.Add(new QuizFactViewModel
                    {
                        MasterId = selectedFact.MasterId,
                        MetadataKey = selectedFact.MetadataKey,
                        MasterName = masterData.Master.Name,
                        MasterImagePath = masterData.PrimaryPicture?.Path ?? "NoImageAvailable.png",
                        AnswerValue = metadataValue.Value,
                        FactText = factText
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging infrastructure
                Console.WriteLine($"Error in GetQuizFacts: {ex.Message}");
                return new List<QuizFactViewModel>();
            }
        }

        /// <summary>
        /// Marks a quiz fact as shown to the user by adding an entry to QuizFactStatus table.
        /// This prevents the same fact from being shown repeatedly.
        /// </summary>
        /// <param name="masterId">The master ID associated with the fact</param>
        /// <param name="metadataKey">The metadata key identifying the fact type</param>
        public void MarkFactAsShown(int masterId, string metadataKey)
        {
            try
            {
                var factStatus = new QuizFactStatus
                {
                    MasterId = masterId,
                    MetadataKey = metadataKey,
                    CreatedDateTime = DateTime.Now
                };

                _userStoreDb.QuizFactStatusRepository.Add(factStatus, checkAlreadyExists: true);
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging infrastructure
                Console.WriteLine($"Error in MarkFactAsShown: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets all shown facts for a specific master or all masters.
        /// Useful for allowing users to see facts again after they've seen all available ones.
        /// </summary>
        /// <param name="masterId">Optional master ID. If null, resets all facts for all masters.</param>
        private static bool IsMeaningfulAnswer(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            var trimmed = value.Trim();
            if (trimmed.Length == 0) return false;
            // Reject values that are only special characters / dashes / punctuation
            if (trimmed.All(c => !char.IsLetterOrDigit(c))) return false;
            // Reject common placeholder patterns like "---", "--", "-"
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"^[-–—]+$")) return false;
            return true;
        }

        public void ResetShownFacts(int? masterId = null)
        {
            try
            {
                var allShownFacts = _userStoreDb.QuizFactStatusRepository.GetAll().ToList();

                if (masterId.HasValue)
                {
                    var factsToDelete = allShownFacts.Where(f => f.MasterId == masterId.Value).ToList();
                    foreach (var fact in factsToDelete)
                    {
                        _userStoreDb.QuizFactStatusRepository.Delete(fact.Id.ToString());
                    }
                }
                else
                {
                    foreach (var fact in allShownFacts.ToList())
                    {
                        _userStoreDb.QuizFactStatusRepository.Delete(fact.Id.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception if you have logging infrastructure
                Console.WriteLine($"Error in ResetShownFacts: {ex.Message}");
            }
        }
    }
}
