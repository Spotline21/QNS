using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using QNS;

namespace accountParser
{
    public class AccountParser
    {
        private static readonly HttpClientHandler handler = new HttpClientHandler { UseCookies = true };
        private static readonly HttpClient Client = new HttpClient(handler); // создаю сессию

        public static async Task <(string LoginLS, string PasswordLS, string FirstLastName, string MacAdressLS)> Parse()
        {
            Console.Write("Введите Номер ЛС Абонента:");
            string AccountNumber = Console.ReadLine();
            var loginUrl = "https://gcdbviewer.matrixhome.net/index.php";

            // ломимся на GCDB
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("login", "spotline"),
                new KeyValuePair<string, string>("password", "zLUnBybz")
            });

            var loginResponse = await Client.PostAsync(loginUrl, loginData);

            if (!loginResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Ошибка Входа в GCDB.");
                return (null, null, null, null);
            }

            var searchUrl = $"https://gcdbviewer.matrixhome.net/usersearch.php?anumber={AccountNumber}&payments_invoices";
            var searchResponse = await Client.GetAsync(searchUrl);
            if (!searchResponse.IsSuccessStatusCode)
            {
                Console.WriteLine("Ошибка при поиске аккаунта.");
                return (null, null, null, null);
            }

            var htmlContent = await searchResponse.Content.ReadAsStringAsync();

            // Сохраняем HTML-страницу в файл для проверки содержимого
            //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "page.html");
            //await File.WriteAllTextAsync(filePath, htmlContent);
            //Console.WriteLine($"HTML-страница сохранена в файл: {filePath}");

            // Парсим с помощью AngleSharp
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(htmlContent));

            // Поиск логина и пароля с помощью CSS-селекторов
            var loginElement = document.QuerySelector("#accounts-box > table > tbody > tr:nth-child(2) > td:nth-child(1) > span.click-select");
            var passwordElement = document.QuerySelector("#accounts-box > table > tbody > tr:nth-child(2) > td:nth-child(2) > span");
            var FirstLastElement = document.QuerySelector("#main > table:nth-child(7) > tbody > tr:nth-child(2) > td:nth-child(1) > div:nth-child(2) > div:nth-child(1) > h3");
            var MacAdressElement = document.QuerySelector("#accounts-box > table > tbody > tr:nth-child(2) > td:nth-child(3) > span");



            for (int i = 0; i <= 100; i += 2) // Процент от 0 до 100
            {
                DisplayProgressBar(i);
                await Task.Delay(5); // Имитация времени выполнения задачи
            }

            Console.WriteLine(); // Переход на новую строку после завершения
            Console.WriteLine("Задача завершена!");

            // Проверяем, найдены ли данные
            if (loginElement == null || passwordElement == null)
            {
                Console.WriteLine("Не удалось найти логин/пароль/MAC-adress в странице абонента.");
                return (null, null, null, null);
            }

            string LoginLS = loginElement.TextContent;
            string PasswordLS = passwordElement.TextContent;
            string FirstLastName = FirstLastElement.TextContent;
            string MacAdressLS = MacAdressElement.TextContent;

            Console.WriteLine("\nДанные подключения: " + FirstLastName + "\n\n");
            Console.WriteLine("Логин абонента: " + LoginLS);
            Console.WriteLine("Пароль по GCDB: " + PasswordLS);
            Console.WriteLine("MAC адрес по GCDB: "+ MacAdressLS);

            Console.WriteLine("\n\n");

            System.Threading.Thread.Sleep(1500);
            return (LoginLS, PasswordLS, FirstLastName, MacAdressLS);
        }

        static public void DisplayProgressBar(int percent)
        {
            int totalBlocks = 50; // Общее количество квадратов
            int filledBlocks = (int)(percent / 100.0 * totalBlocks); // Заполненные квадраты

            // Создаем строку с заполнением и пустыми квадратами
            string progressBar = new string('█', filledBlocks) + new string('░', totalBlocks - filledBlocks);

            // Выводим прогресс с процентом
            Console.CursorLeft = 0; // Ставим курсор в начало строки
            Console.Write($"[{progressBar}] {percent}%");
        }
    }
}
