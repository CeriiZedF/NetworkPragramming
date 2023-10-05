using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Printing;
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
    /// Логика взаимодействия для ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        private Socket? listenSocket;  // "слухаючий" сокет - очікує запитів
        private IPEndPoint? endPoint;  // він "слухає" дану точку

        public ServerWindow()
        {
            InitializeComponent();
        }

        private void SwitchServer_Click(object sender, RoutedEventArgs e)
        {
            if(listenSocket == null)  // сервер OFF, включаємо
            {
                SwapStatus();
                try
                {
                    // парсимо хост - визначаємо номер-адрес вузла з текстового вигляду
                    IPAddress ip = IPAddress.Parse(HostTextBox.Text);
                    // те ж саме з портом
                    int port = Convert.ToInt32(PortTextBox.Text);
                    // збираємо хост + порт в endPoint
                    endPoint = new(ip, port);
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Неправильні параметри конфігураційні" + ex.Message);
                    return;
                }
                listenSocket = new(
                    AddressFamily.InterNetwork,     // IPv4
                    SocketType.Stream,              // Двостороній (читає та пише)
                    ProtocolType.Tcp);              // Протокол - TCP
                // Стартуємо сервер. Оскільки процес слухання довгий (нескінчений)
                // запускаємо його в окремому потоці
                new Thread(StartServer).Start();
            }
            else //сервер ON, виключаємо
            {
                SwapStatus();
                // сервер зупинити, якщо він в очікуванні, дуже складно
                listenSocket.Close();  //створюємо конфлікт - закриваємо сокет
                // це призведе до exception у потоці сервера

            }
        }
        private void StartServer()
        {
            if(listenSocket == null && endPoint == null)
            {
                MessageBox.Show("Спроба запуску без ініціалізації даних ");
                return;
            }

            try
            {
                listenSocket.Bind(endPoint); // якщо порт зайнятий, то буде виключення
                listenSocket.Listen(10); // 10 запитів - максимальна черга, іншим - відмова
                Dispatcher.Invoke(() => ServerLog.Text += "Сервер запущен\n");

                byte[] buffer = new byte[1024]; //буфер прийому даних
                while(true) // нескінчений процес слухання - постійна робота сервера
                {
                    // очікування запиту, саме ця інструкція блокує потік до надходження запиту
                    Socket socket = listenSocket.Accept();

                    // цей код виконується коли сервер отримав запит
                    StringBuilder stringBuilder = new ();
                    do
                    {
                        int n = socket.Receive(buffer);  // отримаємо пакет, зберігамо у буфері
                        // у буфері, n - кількість реально отриманих байт
                        // декодуємо одержані байти у рядок та додаємо до stringBuilder
                        stringBuilder.Append(
                            Encoding.UTF8.GetString(buffer, 0, n));  // TODO: визначити кодування з налаштуванням 
                    }
                    while (socket.Available > 0); 
                    String str = stringBuilder.ToString();
                    Dispatcher.Invoke(() => ServerLog.Text += $"{DateTime.Now} {str}\n");
                }
            }
            catch (Exception ex)
            {
                // Імовірніше за все сервер зупинився кнопкою з UI
                listenSocket = null;
                Dispatcher.Invoke(() => ServerLog.Text += "Сервер зупинено\n");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            listenSocket?.Close();
        }

        private void SwapStatus()
        {
            StatusLabel.Background = StatusLabel.Background == Brushes.Green ? Brushes.Pink : Brushes.Green;
            StatusLabel.Content = StatusLabel.Content.ToString() == "Увімкнено" ? "Вимкнуто" : "Увімкнено";
            SwitchServer.Content = SwitchServer.Content.ToString() == "Включити" ? "Вимкнути" : "Включити";
        }
    }
}
