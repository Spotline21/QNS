using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace accountParser
{
    public class AccountParser
    {
        private static readonly HttpClientHandler handler = new HttpClientHandler { UseCookies = true };

        private static readonly HttpClient Client = new HttpClient(handler); // создаю сессию

        
        public static async Task Parse() 
        {
            
            Console.Write("Введите Номер ЛС Абонента:");
            string AccountNumber = Console.ReadLine();
            var loginUrl = "https://gcdbviewer.matrixhome.net/index.php";


            // ломимся на GCDB
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("login", "spotline"),
                new KeyValuePair<string, string>("password", "zLUnBybz")
            }
            );

            var loginResponse = await Client.PostAsync(loginUrl, loginData);

            if (!loginResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Ошибка Входа в GCDB.");
                return;
            }

            var searchUrl = $"https://gcdbviewer.matrixhome.net/usersearch.php?anumber={AccountNumber}&payments_invoices";
            var searchResponse = await Client.GetAsync(searchUrl);
            if (!searchResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Ошибка при поиске аккаунта.");
                return;
            }

            var htmlContent = await searchResponse.Content.ReadAsStringAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);


            // Сохраняем HTML-страницу в файл
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "page.html");
            await File.WriteAllTextAsync(filePath, htmlContent);
            Console.WriteLine($"HTML-страница сохранена в файл: {filePath}");
            // парсим базу
            var loginNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table/tbody/tr/td/table[1]/tbody/tr[1]/td/div[3]/table/tbody/tr[2]/td[1]/span[1]");
            var passwordNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body/table/tbody/tr/td/table[1]/tbody/tr[1]/td/div[3]/table/tbody/tr[2]/td[2]/span");
            //var LastFirstNamesNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"main\"]/table[1]/tbody/tr[2]/td[1]/div[1]/div[1]/h3");

            // Проверяем, найдены ли данные
            if (loginNode == null || passwordNode == null)
            {
                Console.WriteLine("Не удалось найти логин или пароль.");
                return;
            }
            string LoginLS = loginNode.InnerText;
            string PasswordLS = passwordNode.InnerText;
            // string LFNames = LastFirstNamesNode.InnerText;

            //Console.WriteLine("Зарегистрированно на: " + LFNames + "\n\n");
            Console.WriteLine("Логин абонента: " + LoginLS);
            Console.WriteLine("Пароль по GCDB: " + PasswordLS);

            Console.ReadKey();
        }

    }
}
