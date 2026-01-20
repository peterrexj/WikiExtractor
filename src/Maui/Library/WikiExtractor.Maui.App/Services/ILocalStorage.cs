using Pj.Library.Mobile.DeviceDependency;

namespace WikiExtractor.Maui.App.Services
{
    public interface ILocalStorage
    {
        public ISqlitHelper SqlLiteHelper { get; }
        public ISqlitHelper DbStoreHelper { get; }
    }
}