using NUnit.Framework;
using WikiExtractor.ViewModels;

namespace WikiExtractor.UnitTests;

[TestFixture]
public class BaseViewModelTests
{
    [Test]
    public void IsBusy_SetTrue_IsBusyIsTrue()
    {
        var vm = new BaseViewModel();
        vm.IsBusy = true;
        Assert.That(vm.IsBusy, Is.True);
    }

    [Test]
    public void IsBusy_SetFalse_IsBusyIsFalse()
    {
        var vm = new BaseViewModel();
        vm.IsBusy = true;
        vm.IsBusy = false;
        Assert.That(vm.IsBusy, Is.False);
    }

    [Test]
    public void IsFree_IsInverseOfIsBusy()
    {
        var vm = new BaseViewModel();
        vm.IsBusy = true;
        Assert.That(vm.IsFree, Is.False, "IsFree should be false when IsBusy is true");

        vm.IsBusy = false;
        Assert.That(vm.IsFree, Is.True, "IsFree should be true when IsBusy is false");
    }

    [Test]
    public void IsBusy_PropertyChangedFired_ForBothProperties()
    {
        var vm = new BaseViewModel();
        var changed = new List<string>();
        vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName!);

        vm.IsBusy = true;

        Assert.That(changed, Does.Contain("IsBusy"));
        Assert.That(changed, Does.Contain("IsFree"));
    }
}
