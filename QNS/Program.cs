using System;
using accountParser;

namespace QNS
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await Commands.DrawGreetings();
            await Commands.MenuChanger();

        }
    }

}