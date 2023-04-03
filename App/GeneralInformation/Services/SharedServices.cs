using GeneralInformation.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using WikiExtractor.Process;

namespace GeneralInformation.Services
{
    public class SharedServices
    {
        private static WikiAppController _wikiAppController;
        public static WikiAppController WikiAppController
        {
            get
            {
                _wikiAppController ??= new WikiAppController(DatabaseService.AppDatabase);
                return _wikiAppController;
            }
        }

    }
}
