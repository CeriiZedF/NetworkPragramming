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
using System.Data;

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
            try
            {
               
                using SqlConnection connection = new(ConnectionString);
                connection.Open();

                using SqlCommand command = connection.CreateCommand();
                string code = Guid.NewGuid().ToString()[..6].ToUpper();
                command.CommandText = "INSERT INTO [Users](Email, Password, ConfirmCode) " +
                                      $"VALUES(N'{textBoxEmail.Text}', N'{passwordBox.Password}', '{code}')";
                command.ExecuteNonQuery();

               
                using SmtpClient? smtpClient = GetSmtpClient();
                if (smtpClient is null) { MessageBox.Show("Не удалось подключиться к smtp..."); return; }

                MailMessage mailMessage = new MailMessage(
                    App.GetConfiguration("smtp:email")!,
                    textBoxEmail.Text,
                    "Sign up seccussfull",
                    $"Congratulations! To confirm Email use code: <span style='color: tomato; font-weight: bold;'>{code}</span>"
                )
                { IsBodyHtml = true };

                smtpClient.Send(mailMessage);

                MessageBox.Show("Check Email");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
            if (confirmContainer.Tag is string savedCode)
            {
                if (textBoxCode.Text.Equals(savedCode))
                {
                    using SqlConnection connection = new(ConnectionString);
                    connection.Open();

                    using SqlCommand command = connection.CreateCommand();
                    command.CommandText = $"UPDATE [Users] SET ConfirmCode = NULL " +
                                          $"WHERE [Email] = '{textBoxEmail.Text}' AND [Password] = '{passwordBox.Password}'";
                    command.ExecuteNonQuery();

                    confirmContainer.Visibility = Visibility.Hidden;
                    textBlockLog.Text += "Email confirmed!\n";
                }
                else
                {
                    textBlockLog.Text += "Email not confirmed!\n";
                }
            }
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();

            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM [Users] WHERE [Email] = N'{textBoxEmail.Text}' " +
                                  $"AND [Password] = N'{passwordBox.Password}'";
            using SqlDataReader reader = command.ExecuteReader();
            if (reader.Read()) // пользователь найден
            {
                if (reader.IsDBNull("ConfirmCode"))  // код Null
                {
                    textBlockLog.Text += "Welcome, Email confirmed\n";
                }
                else
                {
                    string code = reader.GetString("ConfirmCode");
                    confirmContainer.Visibility = Visibility.Visible;
                    confirmContainer.Tag = code;
                    textBoxCode.Focus();
                    textBlockLog.Text += "Welcome, Email needs confirmation\n";
                }
            }
            else { MessageBox.Show("Credentials incorrect"); }
        }
    }
}
