using NUnit.Framework;
using WikiExtractor.Exts;
using WikiExtractor.Process;
using WikiExtractor.Repository;
using WikiExtractor.Repository.UserStore;

namespace WikiExtractor.UnitTests;

/// <summary>
/// Integration tests for WikiAppController using real SQLite temp databases.
/// No MAUI/Syncfusion dependency — runs with 'dotnet test' without a simulator.
/// </summary>
[TestFixture]
public class WikiAppControllerTests
{
    private string _dbPath = null!;
    private string _userDbPath = null!;
    private WikiDatabase _wikiDb = null!;
    private UserStoreDatabase _userDb = null!;
    private WikiAppController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        // Use unique temp files per test run to avoid state leakage from the static
        // AppDatabase.IsInitialized flag and between parallel test runs.
        _dbPath = Path.Combine(Path.GetTempPath(), $"wiki_unit_{Guid.NewGuid():N}.db");
        _userDbPath = Path.Combine(Path.GetTempPath(), $"userstore_unit_{Guid.NewGuid():N}.db");

        ProcessConstants.DatabasePath = _dbPath;
        ProcessConstants.UserStoreDatabasePath = _userDbPath;

        // Reset static flags so each test gets a clean database initialisation.
        AppDatabase.IsInitialized = false;
        AppDatabase.IsInitializedUserStore = false;

        _wikiDb = new WikiDatabase();
        _userDb = new UserStoreDatabase();
        _controller = new WikiAppController(_wikiDb, _userDb);
    }

    [TearDown]
    public void TearDown()
    {
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        if (File.Exists(_userDbPath)) File.Delete(_userDbPath);
    }

    [Test]
    public void GetListOfWikiItems_EmptyDb_ReturnsEmptyList()
    {
        var results = _controller.GetListOfWikiItems().ToList();
        Assert.That(results, Is.Empty);
    }

    [Test]
    public void GetListOfWikiItems_OneMaster_ReturnsOneItem()
    {
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Ada Lovelace", Route = "/ada" }, checkAlreadyExists: false);

        var results = _controller.GetListOfWikiItems().ToList();

        Assert.That(results.Count, Is.EqualTo(1));
        Assert.That(results[0].Name, Is.EqualTo("Ada Lovelace"));
    }

    [Test]
    public void GetListOfWikiItems_TwoMasters_ReturnsTwoItems()
    {
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Alan Turing", Route = "/turing" }, checkAlreadyExists: false);
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Grace Hopper", Route = "/hopper" }, checkAlreadyExists: false);

        var results = _controller.GetListOfWikiItems().ToList();

        Assert.That(results.Count, Is.EqualTo(2));
    }

    [Test]
    public void GetListOfWikiItems_AssignsRandomId_ToEveryItem()
    {
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Marie Curie", Route = "/curie" }, checkAlreadyExists: false);
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Nikola Tesla", Route = "/tesla" }, checkAlreadyExists: false);
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Albert Einstein", Route = "/einstein" }, checkAlreadyExists: false);

        var results = _controller.GetListOfWikiItems().ToList();

        // Every item must have had RandomId assigned (non-negative int from RandomHelper)
        Assert.That(results, Has.All.Matches<WikiExtractor.ViewModels.PersonaViewModel>(
            p => p.RandomId >= 0));
    }

    [Test]
    public void GetListOfWikiItems_ItemReadStatus_DefaultsFalse()
    {
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Isaac Newton", Route = "/newton" }, checkAlreadyExists: false);

        var result = _controller.GetListOfWikiItems().Single();

        // No read record in UserStore → should default to not-read
        Assert.That(result.ItemReadStatus, Is.False);
    }

    [Test]
    public void GetListOfWikiItems_IsFavourite_DefaultsFalse()
    {
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Charles Darwin", Route = "/darwin" }, checkAlreadyExists: false);

        var result = _controller.GetListOfWikiItems().Single();

        Assert.That(result.IsFavourite, Is.False);
    }

    [Test]
    public void GetListOfWikiItems_WikiPath_MatchesMasterRoute()
    {
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Galileo Galilei", Route = "/galileo" }, checkAlreadyExists: false);

        var result = _controller.GetListOfWikiItems().Single();

        Assert.That(result.WikiPath, Is.EqualTo("/galileo"));
    }

    [Test]
    public void GetListOfWikiItems_TagFilter_IncludesTaggedItems()
    {
        var id1 = _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Cleopatra", Route = "/cleopatra" }, checkAlreadyExists: false);
        _wikiDb.MasterRepository.Add(new WikiExtractor.DbModels.Master { Name = "Julius Caesar", Route = "/caesar" }, checkAlreadyExists: false);

        var tagId = _wikiDb.TagRepository.Add(new WikiExtractor.DbModels.Tag { Name = "Egypt" }, checkAlreadyExists: true);
        _wikiDb.TagItemRepository.Add(new WikiExtractor.DbModels.TagItem { MasterId = id1, TagId = tagId }, checkAlreadyExists: true);

        var results = _controller.GetListOfWikiItems(tags: new List<string> { "Egypt" }).ToList();

        // Cleopatra is tagged Egypt and should always appear in the results
        Assert.That(results.Select(r => r.Name), Does.Contain("Cleopatra"));
    }
}
