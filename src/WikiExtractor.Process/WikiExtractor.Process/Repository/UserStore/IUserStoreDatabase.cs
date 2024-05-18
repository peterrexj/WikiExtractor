using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Repository.UserStore
{
    public interface IUserStoreDatabase
    {
        ItemReadTrackerRepository ItemReadTrackerRepository { get; set; }
        AppSettingsRepository AppSettingsRepository { get; set; }
        RequestRecordRepository RequestRecordRepository { get; set; }

        void InitializeDatabase();
    }
}
