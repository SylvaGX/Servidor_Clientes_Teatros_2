using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserConnected userConnected { get; }
        List<LogForm> logsForm { get; set; } = new List<LogForm>();
        IEnumerable<LogInfo> logs;
        Timer tickerSessions = new Timer();
        public MainWindow(UserConnected userConnected)
        {
            this.userConnected = userConnected;
            logs = new List<LogInfo>();

            try
            {
                InitializeComponent();

                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client= new LogServiceClient(channel, new LogService.LogServiceClient(channel));

                logs = client.GetAllLogs(userConnected).Result.OrderByDescending(l => l.DataTime);

                if (logs.Any())
                {
                    foreach (LogInfo log in logs)
                    {
                        logsForm.Add(new LogForm()
                        {
                            Id = log.Id,
                            Level = log.LevelLog,
                            Msg = log.Msg,
                            DataLog = new DateTime(log.DataTime),
                        });
                    }
                    LogList.ItemsSource = logsForm;
                }

                channel.ShutdownAsync().Wait();

                tickerSessions.Elapsed += new ElapsedEventHandler(TickerLogs);
                tickerSessions.Interval = 5000; // 1000 ms is one second
                tickerSessions.Start();
            }
            catch (AggregateException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'AggregateException': [{DateTime.Now}] - Error - Erro ao esperar pelo canal fechar.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Main Window", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (ObjectDisposedException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'ObjectDisposedException': [{DateTime.Now}] - Error - Erro ao esperar pelo canal fechar.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Main Window", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullExceptions': [{DateTime.Now}] - Error - Erro ao começar a thread.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Main Window", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (ArgumentNullException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullExceptions': [{DateTime.Now}] - Error - Argumento Nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Main Window", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Main Window", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
        }

        public void TickerLogs(object? source, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here

                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new LogServiceClient(channel, new LogService.LogServiceClient(channel));

                logs = client.GetAllLogs(userConnected).Result.OrderByDescending(l => l.DataTime);

                if (logs.Any())
                {
                    LogList.ItemsSource = null;
                    logsForm.Clear();
                    foreach (LogInfo log in logs)
                    {
                        logsForm.Add(new LogForm()
                        {
                            Id = log.Id,
                            Level = log.LevelLog,
                            Msg = log.Msg,
                            DataLog = new DateTime(log.DataTime),
                        });
                    }
                    LogList.ItemsSource = logsForm;
                }

                channel.ShutdownAsync().Wait();

            }), null);
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void ListTheaters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeatrosList teatrosList = new(userConnected);

                teatrosList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de teatros.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListShows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EspetaculoList espetaculoList = new(userConnected);

                espetaculoList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de espetáculos.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListSessions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SessoesList sessoesList = new(userConnected);

                sessoesList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de sessões.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListPurchases_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComprasList comprasList = new(userConnected);

                comprasList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de compras.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ManagerList managerList = new(userConnected);

                managerList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de managers.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserList userList = new(userConnected);

                userList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de users.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    public class LogForm
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public string Msg { get; set; } = "";
        public DateTime DataLog { get; set; }
    }
}
