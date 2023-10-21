using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NetworkPragramming
{
    /// <summary>
    /// Логика взаимодействия для CryptoWindow.xaml
    /// </summary>
    public partial class CryptoWindow : Window
    {
        private readonly HttpClient _httpClient;
        public ObservableCollection<CoinData> coinsData { get; set; }
        private ListViewItem? prevSelection;

        public CryptoWindow()
        {
            InitializeComponent();
            coinsData = new();
            this.DataContext = this;
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri($"https://api.coincap.io/")
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAssetsAsync();
        }

        private async Task LoadAssetsAsync()
        {
            var responce = JsonSerializer.Deserialize<CoinCapResponce>(
                await _httpClient.GetStringAsync("/v2/assets?limit=10")
            );
            if(responce == null)
            {
                MessageBox.Show("Deserialization Error");
                return;
            }
            coinsData.Clear();
            foreach(var coinData in responce.data)
            {
                coinsData.Add(coinData);
            }
        }

        private void FrameworkElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is ListViewItem item)
            {
                if (prevSelection != null)
                {
                    prevSelection.Background = Brushes.White;
                    
                }
                prevSelection = item;
                item.Background = Brushes.Aqua;
                MessageBox.Show(coinsData[listView.SelectedIndex].id.ToUpper());

            }
            
        }
    }

    #region ORM

    public class CoinData
    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string supply { get; set; }
        public string maxSupply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }
        public string explorer { get; set; }
    }

    public class CoinCapResponce
    {
        public List<CoinData> data { get; set; }
        public long timestamp { get; set; }
    }
    #endregion
}
