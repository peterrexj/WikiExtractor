using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Tests
{
    public class ExtensionMethodsTests
    {
        [TestCase]
        public void Should_Satisfy_FileCreateLogic()
        {
            bool fileExists = true;
            int totalDaysToBeCached = 10;
            var fileCreatedTime = DateTime.Now.AddDays(-12);

            var result = false;
            //if (!fileExists || (fileExists && File.GetCreationTime(_localFileName).AddDays(totalDaysToBeCached) < DateTime.Now))
            if (!fileExists || (fileExists && fileCreatedTime.AddDays(totalDaysToBeCached) < DateTime.Now))
            {
                if (fileExists)
                {
                    result = true;
                }
            }

            Assert.That(result, Is.True);
        }
    }
}
