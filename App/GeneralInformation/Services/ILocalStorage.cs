using Pj.Library.Mobile.DeviceDependency;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralInformation.Services
{
    public interface ILocalStorage
    {
        public ISqlitHelper SqlLiteHelper { get; }
    }
}
