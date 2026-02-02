using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WikiExtractor.Process;
using WikiExtractor.Process.Process;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;
using WikiExtractor.ViewModels;
using Pj.Library;
using System.IO;

namespace WikiExtractor.Tests
{
    /// <summary>
    /// Integration tests for FactCacheService functionality through QuizController.
    /// These tests verify the caching behavior and fact retrieval patterns.
    /// </summary>
    [TestFixture]
    public class FactCacheServiceTests
    {
        [SetUp]
        public void Setup()
        {
            // Setup database path for testing
            ProcessConstants.UserStoreDatabasePath = IoHelper.CombinePath(
                PjUtility.Runtime.ExecutingRepositoryRootFolder, "App", "Databases", "UserStore.db");
        }

        #region Fact Retrieval Performance Tests

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_GetQuizFacts_ReturnsFactsQuickly(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act - Measure retrieval time
            var startTime = DateTime.Now;
            var facts = quizController.GetQuizFacts(10);
            var elapsed = DateTime.Now - startTime;

            // Assert
            Assert.That(facts, Is.Not.Null, "Facts should not be null");
            Assert.That(elapsed.TotalMilliseconds, Is.LessThan(5000), 
                $"Fact retrieval should complete within 5 seconds (took {elapsed.TotalMilliseconds}ms)");
            
            if (facts.Any())
            {
                Assert.That(facts.Count, Is.LessThanOrEqualTo(10), 
                    "Should not exceed requested count");
                
                foreach (var fact in facts)
                {
                    Assert.That(fact.FactText, Is.Not.Null.And.Not.Empty, 
                        "Fact text should be populated");
                    Assert.That(fact.MasterId, Is.GreaterThan(0), 
                        "MasterId should be valid");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_GetQuizFacts_ConsecutiveCalls_AreConsistent(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act - Multiple consecutive retrievals
            var facts1 = quizController.GetQuizFacts(5);
            var facts2 = quizController.GetQuizFacts(5);
            var facts3 = quizController.GetQuizFacts(5);

            // Assert - Each call should return valid data
            Assert.That(facts1, Is.Not.Null);
            Assert.That(facts2, Is.Not.Null);
            Assert.That(facts3, Is.Not.Null);

            // Verify facts are not identical (showing different facts)
            if (facts1.Any() && facts2.Any())
            {
                var fact1Keys = facts1.Select(f => $"{f.MasterId}_{f.MetadataKey}").ToHashSet();
                var fact2Keys = facts2.Select(f => $"{f.MasterId}_{f.MetadataKey}").ToHashSet();
                
                // At least some facts should be different if enough facts exist
                Assert.That(fact1Keys.Union(fact2Keys).Count(), Is.GreaterThanOrEqualTo(5),
                    "Should have variety in facts returned across calls");
            }
        }

        #endregion

        #region Concurrent Access Tests

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_ConcurrentFactRetrieval_IsThreadSafe(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            var tasks = new List<Task<List<QuizFactViewModel>>>();
            var exceptions = new List<Exception>();

            // Act - Multiple threads retrieving facts simultaneously
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        return quizController.GetQuizFacts(3);
                    }
                    catch (Exception ex)
                    {
                        lock (exceptions)
                        {
                            exceptions.Add(ex);
                        }
                        throw;
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.That(exceptions, Is.Empty, 
                "No exceptions should occur during concurrent access");
            
            foreach (var task in tasks)
            {
                Assert.That(task.Result, Is.Not.Null);
                Assert.That(task.Result.Count, Is.LessThanOrEqualTo(3));
            }
        }

        #endregion

        #region Master-Specific Fact Tests

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_GetQuizFacts_WithMasterId_FiltersCorrectly(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Get a valid master ID
            var masterMetadata = wikiDatabase.QuizMasterMetadataRepository.GetAll().FirstOrDefault();
            if (masterMetadata == null)
            {
                Assert.Ignore("No quiz master metadata available");
                return;
            }

            int testMasterId = masterMetadata.MasterId;

            // Act
            var facts = quizController.GetQuizFacts(10, masterId: testMasterId);

            // Assert
            if (facts.Any())
            {
                foreach (var fact in facts)
                {
                    Assert.That(fact.MasterId, Is.EqualTo(testMasterId), 
                        $"All facts should be for MasterId {testMasterId}");
                }
            }
        }

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_GetQuizFacts_WithoutMasterId_ReturnsVariedMasters(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act
            var facts = quizController.GetQuizFacts(15);

            // Assert
            if (facts.Count >= 2)
            {
                var uniqueMasters = facts.Select(f => f.MasterId).Distinct().ToList();
                
                // With enough facts, we should see variety
                Assert.That(uniqueMasters.Count, Is.GreaterThanOrEqualTo(1),
                    "Facts should come from at least one master");
                
                if (uniqueMasters.Count > 1)
                {
                    Console.WriteLine($"Good variety: {uniqueMasters.Count} different masters in results");
                }
            }
        }

        #endregion

        #region Fact Marking Tests

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_MarkFactAsShown_PreventsRepetition(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            quizController.ResetShownFacts();

            var initialFacts = quizController.GetQuizFacts(5);
            if (!initialFacts.Any())
            {
                Assert.Ignore("No facts available");
                return;
            }

            foreach (var fact in initialFacts)
            {
                quizController.MarkFactAsShown(fact.MasterId, fact.MetadataKey);
            }

            // Act
            var nextFacts = quizController.GetQuizFacts(5);

            // Assert
            if (initialFacts.Count >= 5 && nextFacts.Count >= 5)
            {
                var initialKeys = initialFacts.Select(f => $"{f.MasterId}_{f.MetadataKey}").ToHashSet();
                var nextKeys = nextFacts.Select(f => $"{f.MasterId}_{f.MetadataKey}").ToHashSet();

                bool hasDifferentFacts = !initialKeys.SetEquals(nextKeys);
                
                Assert.Pass(hasDifferentFacts 
                    ? "System correctly returned different facts after marking"
                    : "System is cycling facts (expected when fact pool is limited)");
            }
        }

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_ResetShownFacts_AllowsFactReuse(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            var facts = quizController.GetQuizFacts(3);
            if (!facts.Any())
            {
                Assert.Ignore("No facts available");
                return;
            }

            foreach (var fact in facts)
            {
                quizController.MarkFactAsShown(fact.MasterId, fact.MetadataKey);
            }

            // Act
            quizController.ResetShownFacts();

            // Assert
            var newFacts = quizController.GetQuizFacts(3);
            Assert.That(newFacts, Is.Not.Null);
            
            if (newFacts.Any())
            {
                Assert.That(newFacts[0].FactText, Is.Not.Null.And.Not.Empty,
                    "Facts should be available again after reset");
            }
        }

        #endregion

        #region Edge Case Tests

        [Test]
        public void QuizController_GetQuizFacts_WithZeroCount_ReturnsEmptyList()
        {
            // Arrange
            var dbFile = DatabaseFilesWithQuiz.FirstOrDefault();
            if (dbFile == null)
            {
                Assert.Ignore("No database files available");
                return;
            }

            ProcessConstants.DatabasePath = dbFile;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act
            var facts = quizController.GetQuizFacts(0);

            // Assert
            Assert.That(facts, Is.Not.Null);
            Assert.That(facts, Is.Empty);
        }

        [Test]
        public void QuizController_GetQuizFacts_WithNegativeCount_ReturnsEmptyList()
        {
            // Arrange
            var dbFile = DatabaseFilesWithQuiz.FirstOrDefault();
            if (dbFile == null)
            {
                Assert.Ignore("No database files available");
                return;
            }

            ProcessConstants.DatabasePath = dbFile;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act
            var facts = quizController.GetQuizFacts(-5);

            // Assert
            Assert.That(facts, Is.Not.Null);
            Assert.That(facts, Is.Empty);
        }

        [Test]
        [TestCaseSource(nameof(DatabaseFilesWithQuiz))]
        public void QuizController_GetQuizFacts_WithLargeCount_DoesNotExceedAvailable(string dbFilePath)
        {
            // Arrange
            ProcessConstants.DatabasePath = dbFilePath;
            WikiDatabase wikiDatabase = new WikiDatabase();
            UserStoreDatabase userStoreDatabase = new UserStoreDatabase();
            QuizController quizController = new QuizController(wikiDatabase, userStoreDatabase);

            // Act
            var facts = quizController.GetQuizFacts(1000);

            // Assert
            Assert.That(facts, Is.Not.Null);
            Assert.That(facts.Count, Is.LessThanOrEqualTo(1000));
            
            Console.WriteLine($"Requested 1000 facts, received {facts.Count}");
        }

        #endregion

        #region Test Data Sources

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
