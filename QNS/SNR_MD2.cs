using accountParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QNS
{
    public class SNR_MD2
    {
        public static async Task SetupRouter()
        {
            Console.Clear();
            var (LoginLS, PasswordLS, FirstLastName, MacAdressLS) = await AccountParser.Parse();
            AccountParser.DisplayProgressBar(0);
            if (LoginLS == null || PasswordLS == null)
            {
                Console.WriteLine("Не удалось получить данные для настройки сети.");
                return;
            }

            string loginUrl = "http://192.168.1.1/cgi-bin/luci/";
            string configUrl = "http://192.168.1.1/cgi-bin/luci/admin/wifi/main/vif_cfg";

            using (HttpClient httpClient = new HttpClient(new HttpClientHandler { UseCookies = true }))
            {
                // Авторизация
                try
                {
                    var payload = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("luci_username", "Admin"),
                    new KeyValuePair<string, string>("luci_password", "Admin")
                });

                    HttpResponseMessage loginResponse = await httpClient.PostAsync(loginUrl, payload);
                    if (!loginResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Ошибка авторизации.");
                        return;
                    }
                    Console.Clear();
                    Console.WriteLine("Успешная авторизация!");
                    AccountParser.DisplayProgressBar(25);

                    // Запрос пользовательских параметров
                    string ssid1 = LoginLS; // Используем логин для сети 1
                    string password1 = PasswordLS; // Используем пароль для сети 1
                    string ssid2 = LoginLS + "_5G"; // Задайте свое значение для сети 5G
                    string password2 = PasswordLS; // Задайте свое значение для пароля сети 5G

                    // Формирование строки запроса
                    string query = $"?__apply=Сохранить и применить&PMF_2=disable&WPAPSK_2={password2}&EncryptType_2=AES&AuthMode_2=WPA2PSK&TxPower_2=100&BW_2=80&ACSCheckTime_2=12&AutoChannelSelect_2=2&Channel_2=36&WirelessMode_2=14&SSID_2={ssid2}&PMF_1=disable&WPAPSK_1={password1}&EncryptType_1=AES&AuthMode_1=WPA2PSK&TxPower_1=100&BW_1=40&ACSCheckTime_1=12&AutoChannelSelect_1=2&Channel_1=6&WirelessMode_1=7&SSID_1={ssid1}";
                    string url = configUrl + query;

                    // Шаг 2: Отправка настроек
                    HttpResponseMessage configResponse = await httpClient.GetAsync(url);
                    if (configResponse.IsSuccessStatusCode)
                    {
                        Console.Clear();
                        Console.WriteLine("Настройки успешно применены!");
                        AccountParser.DisplayProgressBar(35);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка применения настроек.");
                    }

                    // Отправка первого POST-запроса
                    var firewallPayload = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("token", "19938ce7328544dc289fd40ec87e86f9"),
                    new KeyValuePair<string, string>("cbi.submit", "1"),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.name", "ping"),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.family", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.proto", "icmp"),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.icmp_type", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.src", "wan"),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.src_mac", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.src_ip", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.src_port", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.dest", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.dest_ip", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.dest_port", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.target", "ACCEPT"),
                    new KeyValuePair<string, string>("cbi.cbe.firewall.cfg2092bd.weekdays", "1"),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.start_time", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.stop_time", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.start_date", ""),
                    new KeyValuePair<string, string>("cbid.firewall.cfg2092bd.stop_date", ""),
                    new KeyValuePair<string, string>("cbi.cbe.firewall.cfg2092bd.utc_time", "1")
                });

                    string firewallUrl = "http://192.168.1.1/cgi-bin/luci/admin/network/firewall/rules/cfg2092bd";
                    HttpResponseMessage firewallResponse = await httpClient.PostAsync(firewallUrl, firewallPayload);
                    if (firewallResponse.IsSuccessStatusCode)
                    {
                        Console.Clear();
                        AccountParser.DisplayProgressBar(60);
                        Console.WriteLine("Настройки брандмауэра успешно применены!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка применения настроек брандмауэра.");
                    }

                    // Отправка второго POST-запроса
                    var adminPayload = new FormUrlEncodedContent(new[]
                    {
                    new KeyValuePair<string, string>("token", "19938ce7328544dc289fd40ec87e86f9"),
                    new KeyValuePair<string, string>("cbi.submit", "1"),
                    new KeyValuePair<string, string>("tab.system._config", "user_0"),
                    new KeyValuePair<string, string>("cbid.system._config.u_user_0_name", "Admin"),
                    new KeyValuePair<string, string>("cbid.system._config.u_user_0_pw1", PasswordLS), // Используем пароль из второго метода
                    new KeyValuePair<string, string>("cbid.system._config.u_user_0_pw2", PasswordLS),
                    new KeyValuePair<string, string>("cbi.cbe.system._config.u_user_2_enable", "1"),
                    new KeyValuePair<string, string>("cbid.system._config.u_user_2_name", "User"),
                    new KeyValuePair<string, string>("cbid.system._config.u_user_2_pw1", ""),
                    new KeyValuePair<string, string>("cbid.system._config.u_user_2_pw2", ""),
                    new KeyValuePair<string, string>("cbid.system._remote_config.httpmode", "lanwan"),
                    new KeyValuePair<string, string>("cbid.system._remote_config.httpport", "9090"),
                    new KeyValuePair<string, string>("cbid.system._remote_config.httpsport", "443"),
                    new KeyValuePair<string, string>("cbid.system._remote_config.http_src_ip", ""),
                    new KeyValuePair<string, string>("cbid.system._remote_config.sshmode", "off")
                });

                    string adminUrl = "http://192.168.1.1/cgi-bin/luci/admin/system/admin";
                    HttpResponseMessage adminResponse = await httpClient.PostAsync(adminUrl, adminPayload);
                    if (adminResponse.IsSuccessStatusCode)
                    {
                        Console.Clear();
                        AccountParser.DisplayProgressBar(75);
                        Console.WriteLine("Настройки администратора успешно применены!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка применения настроек администратора.");
                    }

                    // Выполнение финального GET-запроса

                    string saveApplyUrl = "http://192.168.1.1/cgi-bin/luci//admin/uci/saveapply?redir=%2Fcgi-bin%2Fluci%2Fadmin%2Fsystem%2Fadmin";
                    HttpResponseMessage saveApplyResponse = await httpClient.GetAsync(saveApplyUrl);
                    if (saveApplyResponse.IsSuccessStatusCode)
                    {
                        Console.Clear();
                        AccountParser.DisplayProgressBar(100);
                        Console.WriteLine("Настройки успешно сохранены и применены!");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при сохранении и применении настроек.");
                    }
                }



                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }
        }
    }
    
}
