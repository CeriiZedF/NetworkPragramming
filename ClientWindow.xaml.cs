using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Resources.Extensions;
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

namespace NetworkPragramming
{
    /// <summary>
    /// Логика взаимодействия для ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private Random random = new Random();
        private IPEndPoint? endPoint;
        private DateTime lastSyncMoment;
        private bool isServerOn;

        public ClientWindow()
        {
            InitializeComponent();
            lastSyncMoment = default;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "User" + random.Next(100);
            MessageTextBox.Text = "Hello, all!";
            isServerOn = true;
            Sync();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            String[] address = HostTextBox.Text.Split(':');
            try
            {
                endPoint = new(
                    IPAddress.Parse(address[0]),
                    Convert.ToInt32(address[1]));

                new Thread(SendMessage).Start(
                    new ClientRequest
                    {
                        Command = "Message",
                        Message = new()
                        {
                            Login = LoginTextBox.Text,
                            Text = MessageTextBox.Text,
                        }
                    });
                


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
            }
        }

        private async void Sync()
        {
            if (isServerOn)
            {
                String[] address = HostTextBox.Text.Split(':');
                try
                {
                    endPoint = new(
                        IPAddress.Parse(address[0]),
                        Convert.ToInt32(address[1]));

                    new Thread(SendMessage).Start(
                        new ClientRequest
                        {
                            Command = "Check",
                            Message = new()
                            {
                                Moment = lastSyncMoment
                            }
                        }
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            await Task.Delay(1000);
            Sync();
        }

        private void SendMessage(object? arg)
        {
            var clientRequest = arg as ClientRequest;
            
            if (endPoint == null || clientRequest == null)
            {
                return;
            }
            Socket clientSocket = new(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);

            
            try
            {
                clientSocket.Connect(endPoint);   // клієнт "викликає" (сервер слухає)
                
                isServerOn = true;
                clientSocket.Send(
                    Encoding.UTF8.GetBytes(
                       JsonSerializer.Serialize(clientRequest)
                ));
                if (clientRequest.Command == "Message") { Task.Run(() => SwapStatusToInvisible(true)); }
                // Одержуємо відповідь сервера
                MemoryStream memoryStream = new();   // "ByteBuilder" - спосіб накопичити байти
                byte[] buffer = new byte[1024];
                do
                {
                    int n = clientSocket.Receive(buffer);
                    memoryStream.Write(buffer, 0, n);
                } while (clientSocket.Available > 0);
                String str = Encoding.UTF8.GetString(memoryStream.ToArray());

                ServerResponse? response = null;
                try { response = JsonSerializer.Deserialize<ServerResponse>(str); }
                catch { }
                if (response == null)
                {
                    str = "JSON Error in " + str;
                }
                else
                {
                    str = "";
                    // якщо у відповіді є нові повідомлення - виводимо їх
                    ///----------------------------------------------------
                    if (response.messages != null)
                    {
                        foreach (var message in response.messages)
                        {
                            str += message.Moment + "  Asd :   " + message + "\n";
                            // та оновлюємо момент синхронізації
                            if (message.Moment > lastSyncMoment)
                            {
                                lastSyncMoment = message.Moment;
                            }
                        }
                    }
                }
                // Виводимо відповідь на лог
                Dispatcher.Invoke(() => {
                    ClientLog.Text += str;

                    });
                // Повідомляємо сервер про розрив сокету
                clientSocket.Shutdown(SocketShutdown.Both);
                // Звільняємо ресурс
                clientSocket.Dispose();
            }
            catch (Exception ex) {
                if(isServerOn)
                {
                    isServerOn = false;
                    clientSocket.Dispose();
                    Task.Run(() => SwapStatusToInvisible(false));
                    MessageBox.Show(ex.Message);
                    isServerOn = true;
                }
            }
            
        }
        readonly String[] msl = new string[] {"Надіслано", "Помилка"};
        private async Task SwapStatusToInvisible(bool flag)
        {
           

            Dispatcher.Invoke(() =>
            {
                if(flag)
                {
                    StatusLabel.Background = Brushes.Green;
                    StatusLabel.Content = msl[0];
                }
                else
                {
                    StatusLabel.Background = Brushes.Red;
                    StatusLabel.Content = msl[1];
                }
            });
            await Task.Delay(3000);

            Dispatcher.Invoke(() =>
            {
                StatusLabel.Background = Brushes.White;
                StatusLabel.Content = "";
            });
            return;
           
        }
    }
}
