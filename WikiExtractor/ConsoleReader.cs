using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor
{
    internal class ConsoleReader
    {
        public void Read()
        {
            var loopStatus = true;
            while(loopStatus)
            {
                PrintPrimaryOptions();
            }
        }

        private void PrintPrimaryOptions()
        {
            //Console.WriteLine("1. Saints - Extract");
            //Console.WriteLine("2. Saints - Dump Data");

        }
    }
}
