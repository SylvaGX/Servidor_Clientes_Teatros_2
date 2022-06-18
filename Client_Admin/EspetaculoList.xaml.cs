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
    /// Interaction logic for EspetaculoList.xaml
    /// </summary>
    public partial class EspetaculoList : Window
    {
        UserConnected userConnected { get; }
        int VoltarTries = 5;
        List<EspetaculosForm> showsForm { get; set; } = new List<EspetaculosForm>();
        IEnumerable<ShowInfo> shows;

        Timer tickerShows = new Timer();
        public EspetaculoList(UserConnected userConnected)
        {
            this.userConnected = userConnected;
            shows = new List<ShowInfo>();

            try
            {
                InitializeComponent();

                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var clientShow = new ShowServiceClient(channel, new ShowService.ShowServiceClient(channel));

                shows = clientShow.GetShows(userConnected).Result;

                if (shows.Any())
                {
                    foreach (ShowInfo show in shows)
                    {
                        showsForm.Add(new EspetaculosForm()
                        {
                            Id = show.Id,
                            Name = show.Name,
                            StartDate = new DateTime(show.StartDate),
                            EndDate = new DateTime(show.EndDate),
                            Price = Convert.ToDecimal(show.Price),
                            Estado = (show.Estado == 1) ? "Ativo" : "Desativo",
                            EstadoColor = (show.Estado == 1) ? "Green" : "Red",
                        });
                    }
                    ListEspetaculos.ItemsSource = showsForm;
                }

                channel.ShutdownAsync().Wait();

                tickerShows.Elapsed += new ElapsedEventHandler(TickerShows);
                tickerShows.Interval = 5000; // 1000 ms is one second
                tickerShows.Start();
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

                MessageBox.Show("Ocorreu um erro", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch(OverflowException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'OverflowException': [{DateTime.Now}] - Error - Erro ao converter Decimal.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Error);

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
                    Msg = $"'ArgumentOutOfRangeException': [{DateTime.Now}] - Error - Erro ao começar a thread.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
        }

        public void TickerShows(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateShowsList();
            }), null);
        }

        public async void UpdateShowsList()
        {
            try
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new ShowServiceClient(channel, new ShowService.ShowServiceClient(channel));

                shows = client.GetShows(userConnected).Result;

                if (shows.Any())
                {
                    ListEspetaculos.ItemsSource = null;
                    showsForm.Clear();
                    foreach (ShowInfo show in shows)
                    {
                        showsForm.Add(new EspetaculosForm()
                        {
                            Id = show.Id,
                            Name = show.Name,
                            StartDate = new DateTime(show.StartDate),
                            EndDate = new DateTime(show.EndDate),
                            Price = Convert.ToDecimal(show.Price),
                            Estado = (show.Estado == 1) ? "Ativo" : "Desativo",
                            EstadoColor = (show.Estado == 1) ? "Green" : "Red",
                        });
                    }
                    ListEspetaculos.ItemsSource = showsForm;
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

                MessageBox.Show("Ocorreu um erro ao atualizar a lista de espetáculos", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                MessageBox.Show("Ocorreu um erro ao atualizar a lista de espetáculos", "Lista de Teatros", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (OverflowException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'OverflowException': [{DateTime.Now}] - Error - Erro ao converter Decimal.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
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

                MessageBox.Show("Ocorreu um erro ao atualizar a lista de espetáculos", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Voltar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tickerShows.Stop();
                tickerShows.Close();

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

                if (VoltarTries > 0)
                {
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. \nTentativa: {VoltarTries}", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Voltar_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. Poderá não ter ligação à internet. Com isto foi redirecionado para a página de Login", "Lista de Espetáculo", MessageBoxButton.OK, MessageBoxImage.Error);

                    Login login = new();

                    login.Show();

                    Close();
                }
                VoltarTries--;
            }
        }
    }

    public class EspetaculosForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal Price { get; set; }
        public string Estado { get; set; } = "";
        public string EstadoColor { get; set; } = "";
    }
}
