using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NetworkPragramming
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*App.GetConfiguration("smtp:host");*/   // можно замість : -> .
        }
        #region Server
        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            new ServerWindow().Show();
        }
        #endregion

        #region Client
        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            new ClientWindow().Show();
        }
        #endregion

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new EmailWindow().ShowDialog();
            this.Show();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            new AuthWindow().ShowDialog();
        }

        private void HttpButton_Click(object sender, RoutedEventArgs e)
        {
            
            new HttpWindow().ShowDialog();
          
        }

        private void graphCryptoButton_Click(object sender, RoutedEventArgs e)
        {
            new CryptoWindow().ShowDialog();
        }
    }
}