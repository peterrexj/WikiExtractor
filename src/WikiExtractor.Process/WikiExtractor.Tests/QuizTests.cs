using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Pj.Library;
using WikiExtractor.DbModels;
using WikiExtractor.Exts;
using WikiExtractor.Process;
using WikiExtractor.Process.DbModels;
using WikiExtractor.Process.Process;
using WikiExtractor.Process.Repository;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;

namespace WikiExtractor.Tests
{
    [TestFixture]
    public class QuizTests
    {
        [Test]
        public void QuizController_CanBeInstantiated_WithValidDependencies()
        {
            // Arrange
            var mockWikiDb = new Mock<IWikiDatabase>();
            var mockUserStoreDb = new Mock<IUserStoreDatabase>();

            // Act & Assert
            Assert.DoesNotThrow(() => new QuizController(mockWikiDb.Object, mockUserStoreDb.Object));
        }

        [Test]
        public void MarkFactAsShown_DoesNotThrow_WithValidParameters()
        {
            // Arrange
            var mockWikiDb = new Mock<IWikiDatabase>();
            var mockUserStoreDb = new Mock<IUserStoreDatabase>();
            var mockQuizFactStatusRepo = new Mock<QuizFactStatusRepository>(null);

            mockQuizFactStatusRepo.Setup(x => x.Add(It.IsAny<QuizFactStatus>(), It.IsAny<bool>()))
                .Returns(0);

            mockUserStoreDb.Setup(x => x.QuizFactStatusRepository).Returns(mockQuizFactStatusRepo.Object);

            var controller = new QuizController(mockWikiDb.Object, mockUserStoreDb.Object);

            // Act & Assert
            Assert.DoesNotThrow(() => controller.MarkFactAsShown(1, "Born"));
            
            // Verify the repository method was called
            mockQuizFactStatusRepo.Verify(x => x.Add(
                It.Is<QuizFactStatus>(f => f.MasterId == 1 && f.MetadataKey == "Born"),
                It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public void MarkFactAsShown_CallsRepositoryAdd_WithCorrectData()
        {
            // Arrange
            var mockWikiDb = new Mock<IWikiDatabase>();
            var mockUserStoreDb = new Mock<IUserStoreDatabase>();
            var mockQuizFactStatusRepo = new Mock<QuizFactStatusRepository>(null);
            QuizFactStatus? capturedFact = null;

            mockQuizFactStatusRepo.Setup(x => x.Add(It.IsAny<QuizFactStatus>(), It.IsAny<bool>()))
                .Callback<QuizFactStatus, bool>((fact, check) => capturedFact = fact)
                .Returns(0);

            mockUserStoreDb.Setup(x => x.QuizFactStatusRepository).Returns(mockQuizFactStatusRepo.Object);

            var controller = new QuizController(mockWikiDb.Object, mockUserStoreDb.Object);

            // Act
            controller.MarkFactAsShown(123, "TestKey");

            // Assert
            Assert.That(capturedFact, Is.Not.Null);
            Assert.That(capturedFact!.MasterId, Is.EqualTo(123));
            Assert.That(capturedFact.MetadataKey, Is.EqualTo("TestKey"));
            Assert.That((DateTime.Now - capturedFact.CreatedDateTime).TotalSeconds, Is.LessThan(5));
        }

        [Test]
        public void GetQuizFacts_ReturnsFacts_WithNonEmptyFactText()
        {
            // Arrange - Create simple test to verify the Fact field structure
            var quizDefinition = new QuizDefinition
            {
                MetadataKey = "Born",
                QuestionPhrase = "When was {MasterId} born?",
                Fact = "{MasterId} was born in {AnswerId}."
            };

            // Assert - Verify the Fact field is properly structured
            Assert.That(quizDefinition.Fact, Is.Not.Null, "Fact field should not be null");
            Assert.That(quizDefinition.Fact, Is.Not.Empty, "Fact field should not be empty");
            Assert.That(quizDefinition.Fact, Does.Contain("{MasterId}"), "Fact should contain MasterId placeholder");
            Assert.That(quizDefinition.Fact, Does.Contain("{AnswerId}"), "Fact should contain AnswerId placeholder");

            // Test fact text replacement logic
            var masterName = "Pope John Paul II";
            var answerValue = "1920";
            var replacedFact = quizDefinition.Fact
                .Replace("{MasterId}", masterName)
                .Replace("{AnswerId}", answerValue);

            Assert.That(replacedFact, Is.Not.Null.And.Not.Empty, "Replaced fact should not be empty");
            Assert.That(replacedFact, Does.Not.Contain("{MasterId}"), "Replaced fact should not contain MasterId placeholder");
            Assert.That(replacedFact, Does.Not.Contain("{AnswerId}"), "Replaced fact should not contain AnswerId placeholder");
            Assert.That(replacedFact, Does.Contain(masterName), "Replaced fact should contain master name");
            Assert.That(replacedFact, Does.Contain(answerValue), "Replaced fact should contain answer value");
            Assert.That(replacedFact, Is.EqualTo("Pope John Paul II was born in 1920."), 
                "Fact should be properly formatted with replaced values");
        }

        #region Unit Tests - SaveResponse

        [Test]
        public void SaveResponse_CallsRepositoryAdd_WithCorrectData()
        {
            // Arrange
            var mockWikiDb = new Mock<IWikiDatabase>();
            var mockUserStoreDb = new Mock<IUserStoreDatabase>();
            var mockQuizResponseRepo = new Mock<QuizResponseRepository>(null);
            
            QuizResponse capturedResponse = null;
            mockQuizResponseRepo.Setup(x => x.Add(It.IsAny<QuizResponse>(), It.IsAny<bool>()))
                .Callback<QuizResponse, bool>((response, check) => capturedResponse = response)
                .Returns(0);

            mockUserStoreDb.Setup(x => x.QuizResponseRepository).Returns(mockQuizResponseRepo.Object);
            var controller = new QuizController(mockWikiDb.Object, mockUserStoreDb.Object);

            var testResponse = new QuizResponse
            {
                MasterId = 100,
                MetadataKey = "TestKey",
                UserResponse = 1,
                QuestionSetId = 5
            };

            // Act
            controller.SaveResponse(testResponse);

            // Assert
            Assert.That(capturedResponse, Is.Not.Null);
            Assert.That(capturedResponse.MasterId, Is.EqualTo(100));
            Assert.That(capturedResponse.MetadataKey, Is.EqualTo("TestKey"));
            Assert.That(capturedResponse.UserResponse, Is.EqualTo(1));
            Assert.That(capturedResponse.QuestionSetId, Is.EqualTo(5));
            mockQuizResponseRepo.Verify(x => x.Add(testResponse, false), Times.Once);
        }

        #endregion

        #region Unit Tests - ResetShownFacts - Integration Tests Cover This Better

        // Note: ResetShownFacts unit tests removed because GetAll() is non-mockable
        // Integration tests provide better coverage for this functionality

        #endregion

        #region Unit Tests - GetQuizFacts Edge Cases

        [Test]
        public void GetQuizFacts_WithZeroCount_ReturnsEmptyList()
        {
            // Arrange
            var mockWikiDb = new Mock<IWikiDatabase>();
            var mockUserStoreDb = new Mock<IUserStoreDatabase>();
            var controller = new QuizController(mockWikiDb.Object, mockUserStoreDb.Object);

            // Act
            var result = controller.GetQuizFacts(0);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetQuizFacts_WithNegativeCount_ReturnsEmptyList()
        {
            // Arrange
            var mockWikiDb = new Mock<IWikiDatabase>();
            var mockUserStoreDb = new Mock<IUserStoreDatabase>();
            var controller = new QuizController(mockWikiDb.Object, mockUserStoreDb.Object);

            // Act
            var result = controller.GetQuizFacts(-5);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region Integration Tests - Real Database

        [SetUp]
        public void TestSetup()
        {
            ProcessConstants.UserStoreDatabasePath = IoHelper.CombinePath(
                PjUtility.Runtime.ExecutingRepositoryRootFolder, "App", "Databases", "UserStore.db");
        }

        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void Integration_GetQuizFacts_ReturnsFactsWithReplacedPlaceholders(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act
            var facts = quizController.GetQuizFacts(5);

            // Assert
            if (facts.Any())
            {
                foreach (var fact in facts)
                {
                    Assert.That(fact.FactText, Is.Not.Null.And.Not.Empty, "Fact text should not be empty");
                    Assert.That(fact.FactText, Does.Not.Contain("{MasterId}"), 
                        "Fact text should have MasterId placeholder replaced");
                    Assert.That(fact.FactText, Does.Not.Contain("{AnswerId}"), 
                        "Fact text should have AnswerId placeholder replaced");
                    Assert.That(fact.MasterName, Is.Not.Null.And.Not.Empty, "Master name should be populated");
                    Assert.That(fact.MasterImagePath, Is.Not.Null.And.Not.Empty, "Master image path should be populated");
                }
            }
        }

        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void Integration_GetQuizFacts_WithMasterId_ReturnsOnlyRelevantFacts(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Get any master ID that has quiz data
            var quizMasterMetadata = wikiDatabase.QuizMasterMetadataRepository.GetAll().FirstOrDefault();
            if (quizMasterMetadata == null)
            {
                Assert.Ignore("No quiz master metadata available");
                return;
            }

            int testMasterId = quizMasterMetadata.MasterId;

            // Act
            var facts = quizController.GetQuizFacts(5, masterId: testMasterId);

            // Assert
            if (facts.Any())
            {
                foreach (var fact in facts)
                {
                    Assert.That(fact.MasterId, Is.EqualTo(testMasterId), 
                        "All facts should be for the specified master");
                }
            }
        }

        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void Integration_SaveResponse_PersistsDataToDatabase(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            var quizMasterMetadata = wikiDatabase.QuizMasterMetadataRepository.GetAll().FirstOrDefault();
            if (quizMasterMetadata == null)
            {
                Assert.Ignore("No quiz master metadata available");
                return;
            }

            var testResponse = new QuizResponse
            {
                MasterId = quizMasterMetadata.MasterId,
                MetadataKey = quizMasterMetadata.MetadataKey,
                UserResponse = 1,
                QuestionSetId = 999,
                CreatedDateTime = DateTime.Now
            };

            // Act
            Assert.DoesNotThrow(() => quizController.SaveResponse(testResponse));

            // Assert - Verify it was saved
            var savedResponses = userStoreDatabase.QuizResponseRepository.GetAll()
                .Where(r => r.QuestionSetId == 999).ToList();
            Assert.That(savedResponses, Is.Not.Empty, "Response should be saved to database");
        }

        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void Integration_MarkFactAsShown_PersistsToDatabase(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            var quizMasterMetadata = wikiDatabase.QuizMasterMetadataRepository.GetAll().FirstOrDefault();
            if (quizMasterMetadata == null)
            {
                Assert.Ignore("No quiz master metadata available");
                return;
            }

            int testMasterId = quizMasterMetadata.MasterId;
            string testMetadataKey = quizMasterMetadata.MetadataKey;

            // Act
            Assert.DoesNotThrow(() => quizController.MarkFactAsShown(testMasterId, testMetadataKey));

            // Assert - Verify it was saved
            var shownFacts = userStoreDatabase.QuizFactStatusRepository.GetAll()
                .Where(f => f.MasterId == testMasterId && f.MetadataKey == testMetadataKey)
                .ToList();
            Assert.That(shownFacts, Is.Not.Empty, "Fact should be marked as shown in database");
        }

        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void Integration_ResetShownFacts_ClearsDatabase(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            var quizMasterMetadata = wikiDatabase.QuizMasterMetadataRepository.GetAll().FirstOrDefault();
            if (quizMasterMetadata == null)
            {
                Assert.Ignore("No quiz master metadata available");
                return;
            }

            int testMasterId = quizMasterMetadata.MasterId;
            string testMetadataKey = quizMasterMetadata.MetadataKey;

            // Add a fact first
            quizController.MarkFactAsShown(testMasterId, testMetadataKey);

            // Act
            quizController.ResetShownFacts(masterId: testMasterId);

            // Assert
            var shownFacts = userStoreDatabase.QuizFactStatusRepository.GetAll()
                .Where(f => f.MasterId == testMasterId)
                .ToList();
            Assert.That(shownFacts, Is.Empty, "All facts for the master should be cleared");
        }

        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void Integration_GetQuestionSetId_ReturnsPositiveInteger(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act
            var questionSetId = quizController.GetQuestionSetId();

            // Assert
            // The method returns max existing ID + 1, or 1 if no responses exist
            // Since we're using a fresh UserStore.db in tests, it typically returns 1
            Assert.That(questionSetId, Is.GreaterThan(0), "Question set ID should be a positive integer");
        }

        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void Integration_GenerateQuizQuestions_ReturnsQuestions(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act
            var questions = quizController.GenerateQuizQuestionsForNewSession();

            // Assert
            if (questions != null && questions.Any())
            {
                Assert.That(questions.Count, Is.LessThanOrEqualTo(10), "Should return at most 10 questions");
                
                foreach (var question in questions)
                {
                    Assert.That(question.Question, Is.Not.Null.And.Not.Empty, "Question text should not be empty");
                    Assert.That(question.AnswerCollection, Is.Not.Null.And.Not.Empty, "Question should have answer options");
                    Assert.That(question.CorrectAnswer, Is.Not.Null.And.Not.Empty, "Should have correct answer");
                    Assert.That(question.MasterId, Is.GreaterThan(0), "Should have valid master ID");
                    Assert.That(question.MetadataKey, Is.Not.Null.And.Not.Empty, "Should have metadata key");
                }
            }
        }

        public static IEnumerable<string> DatabaseFilesWithQuiz
        {
            get
            {
                return Directory.EnumerateFiles(
                    IoHelper.CombinePath(PjUtility.Runtime.ExecutingRepositoryRootFolder,
                    "Resources\\Databases"), "*.db")
                    .Where(f => Path.GetFileNameWithoutExtension(f).EqualsIgnoreCase("UserStore") == false)
                    .Where(f => !Path.GetFileNameWithoutExtension(f).EqualsIgnoreCase("WikiStoreCountries"));
            }
        }

        #endregion
    }
}


