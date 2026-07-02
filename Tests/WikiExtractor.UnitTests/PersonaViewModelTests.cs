using NUnit.Framework;
using WikiExtractor.ViewModels;

namespace WikiExtractor.UnitTests;

[TestFixture]
public class PersonaViewModelTests
{
    [Test]
    public void PrimaryMetadataFormatted_EmptyList_ReturnsEmptyString()
    {
        var vm = new PersonaViewModel { PrimaryMetadataContent = new List<MetadataViewModel>() };
        Assert.That(vm.PrimaryMetadataFormatted, Is.EqualTo(string.Empty));
    }

    [Test]
    public void PrimaryMetadataFormatted_NullList_ReturnsEmptyString()
    {
        var vm = new PersonaViewModel();
        vm.PrimaryMetadataContent = null;
        Assert.That(vm.PrimaryMetadataFormatted, Is.EqualTo(string.Empty));
    }

    [Test]
    public void PrimaryMetadataFormatted_SingleItem_ReturnsKeyColonDescription()
    {
        var vm = new PersonaViewModel
        {
            PrimaryMetadataContent = new List<MetadataViewModel>
            {
                new() { Key = "Born", Description = "1 Jan 1970" }
            }
        };
        Assert.That(vm.PrimaryMetadataFormatted, Is.EqualTo("Born: 1 Jan 1970"));
    }

    [Test]
    public void PrimaryMetadataFormatted_MultipleItems_JoinedWithNewline()
    {
        var vm = new PersonaViewModel
        {
            PrimaryMetadataContent = new List<MetadataViewModel>
            {
                new() { Key = "Born", Description = "1 Jan 1970" },
                new() { Key = "Country", Description = "France" },
            }
        };
        Assert.That(vm.PrimaryMetadataFormatted, Is.EqualTo("Born: 1 Jan 1970\nCountry: France"));
    }

    [Test]
    public void PrimaryMetadataFormatted_IsCached_SameReferenceOnSecondAccess()
    {
        var vm = new PersonaViewModel
        {
            PrimaryMetadataContent = new List<MetadataViewModel>
            {
                new() { Key = "Born", Description = "1 Jan 1970" }
            }
        };
        var first = vm.PrimaryMetadataFormatted;
        var second = vm.PrimaryMetadataFormatted;
        Assert.That(ReferenceEquals(first, second), Is.True, "Should return the same cached string instance");
    }

    [Test]
    public void PrimaryMetadataFormatted_CacheInvalidatedOnContentSet()
    {
        var vm = new PersonaViewModel
        {
            PrimaryMetadataContent = new List<MetadataViewModel>
            {
                new() { Key = "Born", Description = "1 Jan 1970" }
            }
        };
        var first = vm.PrimaryMetadataFormatted;

        vm.PrimaryMetadataContent = new List<MetadataViewModel>
        {
            new() { Key = "Country", Description = "France" }
        };
        var second = vm.PrimaryMetadataFormatted;

        Assert.That(second, Is.EqualTo("Country: France"));
        Assert.That(first, Is.EqualTo("Born: 1 Jan 1970"));
    }
}
