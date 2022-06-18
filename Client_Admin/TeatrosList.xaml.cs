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
using System.Windows.Shapes;

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for TeatrosList.xaml
    /// </summary>
    public partial class TeatrosList : Window
    {
        UserConnected userConnected { get; }
        int VoltarTries = 5;
        List<TeatrosForm> teatrosForm { get; set; } = new List<TeatrosForm>();
        IEnumerable<TheaterInfo> theaters;

        Timer tickerTheaters = new Timer();
        public TeatrosList(UserConnected userConnected)
        {
            this.userConnected = userConnected;
            theaters = new List<TheaterInfo>();
            try
            {
                InitializeComponent();

                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var clientTheater = new TheaterServiceClient(channel, new TheaterService.TheaterServiceClient(channel));

                theaters = clientTheater.GetTheaters(userConnected).Result;

                if (theaters.Any())
                {
                    foreach (TheaterInfo theater in theaters)
                    {
                        teatrosForm.Add(new TeatrosForm()
                        {
                            Id = theater.Id,
                            Name = theater.Name,
                            Loc = theater.Localization.Name,
                            Estado = (theater.Estado == 1) ? "Ativo" : "Desativo",
                            EstadoColor = (theater.Estado == 1) ? "Green" : "Red",
                        });
                    }
                    ListTeatros.ItemsSource = teatrosForm;
                }

                channel.ShutdownAsync().Wait();

                tickerTheaters.Elapsed += new ElapsedEventHandler(TickerTheaters);
                tickerTheaters.Interval = 5000; // 1000 ms is one second
                tickerTheaters.Start();
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

                MessageBox.Show("Ocorreu um erro", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new (userConnected);
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

                MessageBox.Show("Ocorreu um erro", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch(ArgumentOutOfRangeException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentOutOfRangeException': [{DateTime.Now}] - Error - Erro ao começar a thread.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
        }

        public void TickerTheaters(object? source, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateTeatrosList();
            }), null);
        }

        public async void UpdateTeatrosList()
        {
            try
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new TheaterServiceClient(channel, new TheaterService.TheaterServiceClient(channel));

                theaters = client.GetTheaters(userConnected).Result;

                if (theaters.Any())
                {
                    ListTeatros.ItemsSource = null;
                    teatrosForm.Clear();
                    foreach (TheaterInfo theater in theaters)
                    {
                        teatrosForm.Add(new TeatrosForm()
                        {
                            Id = theater.Id,
                            Name = theater.Name,
                            Loc = theater.Localization.Name,
                            Estado = (theater.Estado == 1) ? "Ativo" : "Desativo",
                            EstadoColor = (theater.Estado == 1) ? "Green" : "Red",
                        });
                    }
                    ListTeatros.ItemsSource = teatrosForm;
                }

                channel.ShutdownAsync().Wait();
            }
            catch (AggregateException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'AggregateException': [{DateTime.Now}] - Error - Erro ao esperar pelo canal fechar.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro ao atualizar o teatro", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (ObjectDisposedException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'ObjectDisposedException': [{DateTime.Now}] - Error - Erro ao esperar pelo canal fechar.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro ao atualizar o teatro", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch(Exception ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro ao atualizar o teatro", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Voltar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tickerTheaters.Stop();
                tickerTheaters.Close();

                MainWindow mainWindow = new MainWindow(userConnected);

                mainWindow.Show();

                Close();
            }
            catch (Exception ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                if(VoltarTries > 0)
                {
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. \nTentativa: {VoltarTries}", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Voltar_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. Poderá não ter ligação à internet. Com isto foi redirecionado para a página de Login", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Error);

                    Login login = new ();

                    login.Show();

                    Close();
                }
                VoltarTries--;
            }
        }
    }
    public class TeatrosForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Loc { get; set; } = "";
        public string Estado { get; set; } = "";
        public string EstadoColor { get; set; } = "";
    }
}
