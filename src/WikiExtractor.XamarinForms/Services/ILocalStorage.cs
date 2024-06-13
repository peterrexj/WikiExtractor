using Pj.Library.Mobile.DeviceDependency;

namespace GeneralInformation.Services
{
    public interface ILocalStorage
    {
        public ISqlitHelper SqlLiteHelper { get; }
    }
}
