using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private String ConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=D:\\Repos\\NetworkPragramming\\Database.mdf;Integrated Security=True";
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            using System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            String code = Guid.NewGuid().ToString()[..6].ToUpper();
            command.CommandText = 
                "INSERT INTO Users(Email, Password, ConfirmCode)" +
                $" VALUES (N'{textBoxEmail.Text}', N'{textBoxPassword.Text}', '{code}')";
            command.ExecuteNonQuery();

            using SmtpClient smtpClient = GetSmtpClient();
            smtpClient?.Send(
                App.GetConfiguration("smtp:email"),
                textBoxEmail.Text,
                "SignUp SuccessFull",
                $"Congratulations! To confirm Email use code: {code}");

            MessageBox.Show("Check Email!!!");
        }

        private SmtpClient? GetSmtpClient()
        {
            #region get and check config
            String? host = App.GetConfiguration("smtp:host");
            if (host == null)
            {
                MessageBox.Show("Error getting host");
                return null;
            }
            String? portString = App.GetConfiguration("smtp:port");
            if (portString == null)
            {
                MessageBox.Show("Error getting port");
                return null;
            }
            int port;
            try { port = int.Parse(portString); }
            catch
            {
                MessageBox.Show("Error parsing port");
                return null;
            }
            String? email = App.GetConfiguration("smtp:email");
            if (email == null)
            {
                MessageBox.Show("Error getting email");
                return null;
            }
            String? password = App.GetConfiguration("smtp:password");
            if (password == null)
            {
                MessageBox.Show("Error getting password");
                return null;
            }
            String? sslString = App.GetConfiguration("smtp:ssl");
            if (sslString == null)
            {
                MessageBox.Show("Error getting ssl");
                return null;
            }
            bool ssl;
            try { ssl = bool.Parse(sslString); }
            catch
            {
                MessageBox.Show("Error parsing ssl");
                return null;
            }
            #endregion

            return new(host, port)
            {
                EnableSsl = ssl,
                Credentials = new NetworkCredential(email, password)
            };
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if(ConfirmContainer.Tag is String savedCode)
            {
                if(savedCode.Equals(savedCode))
                {
                    LogTextBlock.Text += "Email confirmed\n";
                }
                else
                {
                    LogTextBlock.Text += "Email not confirmed\n";
                }
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            using System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(ConnectionString);
            connection.Open();
            using SqlCommand command = connection.CreateCommand();
            String code = Guid.NewGuid().ToString()[..6].ToUpper();
            command.CommandText =
                "SELECT * FROM [Users] WHERE " +
                $"[Email]=N'{textBoxEmail.Text}' " +
                $"AND [Password]=N'{textBoxPassword.Text}' ";
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                if(!reader.IsDBNull(2))
                {
                    String codes = reader.GetString(2);
                    ConfirmContainer.Visibility = Visibility.Visible;
                    textBoxCode.Focus();
                    LogTextBlock.Text += "Welcome, Email needs confirmation\n";
                }
                else
                {
                    LogTextBlock.Text += "Welcome, Email confirmed\n";
                }
            }
            else
            {
                MessageBox.Show("Credentials incorrect");
            }
        }
    }
}
