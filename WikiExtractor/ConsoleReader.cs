using Pj.Library;
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

                Console.Write("Enter your option: ");
                var userInput = Console.ReadLine().ToInteger();
            }
        }

        private void PrintPrimaryOptions()
        {
            Console.WriteLine("1. Country Wiki Extract");
            Console.WriteLine("2. Saints Wiki Extract");
            Console.WriteLine("3. Popes Wiki Extract");
            Console.WriteLine("9. Exit");
        }
    }
}
