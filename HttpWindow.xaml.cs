using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkPragramming
{
    /// <summary>
    /// Логика взаимодействия для HttpWindow.xaml
    /// </summary>
    public partial class HttpWindow : Window
    {
        private List<NbuRate>? rates;
        private String[] popularCc = { "XAU", "USD", "EUR" };

        public HttpWindow()
        {
            InitializeComponent();
        }

        private async void get1Button_Click(object sender, RoutedEventArgs e)
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync("https://itstep.org/");
            scrollViewer.Content = "";
            scrollViewer.Content += (int)response.StatusCode +
                " " + response.ReasonPhrase + "\r\n";
            foreach (var header in response.Headers)
            {
                scrollViewer.Content += $"{header.Key,-20}: "
                    + String.Join(',', header.Value).Ellipsis(30)
                    + "\r\n";
            }
            String body = await response.Content.ReadAsStringAsync();
            scrollViewer.Content += $"\r\n{body}";
            /* Д.З. Додати до вікна HttpWindow дві кнопки, які
             * завантажують дані про курси валют в різних форматах (XML, JSON)
             * https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange
             * https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json
             * та виводять їх у вигляді тексту на інтерфейс вікна.
             * Додати скріншоти
             */
        }

        private async void ratesButton_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.Content = "";
            if (rates == null)
            {
                await loadRatesAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            }
            if (rates == null)
            {
                return;
            }
            foreach (var rate in rates)
            {
                scrollViewer.Content += $"{rate.cc} {rate.txt} {rate.rate}\n";
            }
        }
        private async Task loadRatesAsync(String uri)
        {
            using HttpClient httpClient = new();
            String body = await httpClient.GetStringAsync(uri);
            // ORM (у даному контексті) - переведення String body в об'єкти
            rates = JsonSerializer.Deserialize<List<NbuRate>>(body);
            if (rates == null)
            {
                MessageBox.Show("Error deserializing");
                return;
            }
        }

        private async void popularButton_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.Content = "";
            if (rates == null)
            {
                await loadRatesAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            }
            if (rates == null)
            {
                return;
            }
            foreach (var rate in rates)
            {
                if (popularCc.Contains(rate.cc))
                {
                    scrollViewer.Content += $"{rate.cc} {rate.txt} {rate.rate}\n";
                }
            }
        }

        #region XML and JSON
        private async void xmlButton_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.Content = "";
            using HttpClient httpClient = new();
            String body = await httpClient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange");
            if (body == null)
            {
                MessageBox.Show("Null");
                return;
            }
            scrollViewer.Content = body;
        }

        private async void jsonButton_Click(object sender, RoutedEventArgs e)
        {
            scrollViewer.Content = "";
            using HttpClient httpClient = new();
            String body = await httpClient.GetStringAsync(@"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json");
            if (body == null)
            {
                MessageBox.Show("Null");
                return;
            }
            scrollViewer.Content = body;
        }
        #endregion 
    }

    public class NbuRate
    {
        public int r030 { get; set; }
        public String txt { get; set; }
        public double rate { get; set; }
        public String cc { get; set; }
        public String exchangeDate { get; set; }
    }


    public static class EllipsisExtensions
    {
        public static string Ellipsis(this string str, int maxLength)
        {
            return str.Length > maxLength ? str[..(maxLength - 3)] + "..." : str;
        }
    }

}
