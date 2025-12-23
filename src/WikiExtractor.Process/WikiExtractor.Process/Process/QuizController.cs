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

            int questionCount = 3; //TODO: Change to 10 or configurable
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

                var randomAnswers = RandomGeneratorHelper.RandomizeSubset(
                    metadataRawDb
                        .Where(f => f.Key.EqualsIgnoreCase(randomQuestion.MetadataKey) && f.Value != correctAnswer)
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
                                          g.quiz.MaxLengthForAnswer == 0 ||
                                          g.f.Value.Length <= g.quiz.MaxLengthForAnswer)
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
                    { MetadataKey = metadata, QuestionPhrase = dataDefinition?.QuestionRephrase }, checkAlreadyExists: true);
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
    }
}
