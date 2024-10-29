using System;
using accountParser;

namespace QNS
{
    class Program
    {
        public static async Task Main(string[] args)
        {

            AccountParser.Parse();

            Console.ReadKey();
        }
    }

}