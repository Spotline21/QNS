using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNS
{
    public static class Commands
    {
        public static async Task DrawGreetings()
        {
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine(@"   ____  _   _  _____                         
  / __ \| \ | |/ ____|                        
 | |  | |  \| | (___                          
 | |  | | . ` |\___ \                         
 | |__| | |\  |____) |                        
  \___\_\_| \_|_____/         _               
  / _|        | |            | |              
 | |_ __ _ ___| |_   ___  ___| |_ _   _ _ __  
 |  _/ _` / __| __| / __|/ _ \ __| | | | '_ \ 
 | || (_| \__ \ |_  \__ \  __/ |_| |_| | |_) |
 |_| \__,_|___/\__| |___/\___|\__|\__,_| .__/ 
                                       | |    
                                       |_|  ");
            Console.WriteLine("\n для продолжения нажмите любую клавишу");

        }

        public static async Task MenuChanger()
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Читаем нажатую клавишу, но не отображаем её в консоли


                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n\nнажмите 1, чтобы начать автонастройку роутера");
                Console.WriteLine("Нажмите Q, чтобы выйти из программы");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Нажмите alt+a чтобы перейти в режим разработчика");
                Console.ForegroundColor = ConsoleColor.White;

                if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt) && keyInfo.Key == ConsoleKey.A)
                {
                    Console.Clear();
                    Console.WriteLine("вы вошли в режим разработчика");
                }
                else if (keyInfo.Key == ConsoleKey.D1)
                {
                    Console.Clear();

                    await accountParser.AccountParser.Parse();
                }
                else if (keyInfo.Key == ConsoleKey.Q)
                {
                    Console.Clear();

                    Console.WriteLine("закрываю...");
                    System.Threading.Thread.Sleep(2000);
                    break;
                }
            }

        }
    }
}
