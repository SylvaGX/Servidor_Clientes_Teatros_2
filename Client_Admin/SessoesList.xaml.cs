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
    /// Interaction logic for SessoesList.xaml
    /// </summary>
    public partial class SessoesList : Window
    {
        UserConnected userConnected { get; }
        int VoltarTries = 5;
        List<SessoesForm> sessionsForm { get; set; } = new List<SessoesForm>();
        IEnumerable<SessionInfo> sessions;

        Timer tickerSessions = new Timer();
        public SessoesList(UserConnected userConnected)
        {
            this.userConnected = userConnected;
            sessions = new List<SessionInfo>();

            try
            {
                InitializeComponent();

                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var clientSessions = new SessionServiceClient(channel, new SessionService.SessionServiceClient(channel));

                sessions = clientSessions.GetAllSessions(userConnected).Result;

                if (sessions.Any())
                {
                    foreach (SessionInfo session in sessions)
                    {
                        sessionsForm.Add(new SessoesForm()
                        {
                            Id = session.Id,
                            SessionDate = new DateTime(session.SessionDate),
                            StartHour = new TimeSpan(session.StartHour),
                            EndHour = new TimeSpan(session.EndHour),
                            TotalPlaces = session.TotalPlaces,
                            nameShow = session.Show.Name,
                            Estado = (session.Estado == 1) ? "Ativo" : "Desativo",
                            EstadoColor = (session.Estado == 1) ? "Green" : "Red",
                        });
                    }
                    ListSessoes.ItemsSource = sessionsForm;
                }

                channel.ShutdownAsync().Wait();

                tickerSessions.Elapsed += new ElapsedEventHandler(TickerSessions);
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

                MessageBox.Show("Ocorreu um erro", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
        }

        public void TickerSessions(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateSessionsList();
            }), null);
        }

        public async void UpdateSessionsList()
        {
            try
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new SessionServiceClient(channel, new SessionService.SessionServiceClient(channel));

                sessions = client.GetAllSessions(userConnected).Result;

                if (sessions.Any())
                {
                    ListSessoes.ItemsSource = null;
                    sessionsForm.Clear();
                    foreach (SessionInfo session in sessions)
                    {
                        sessionsForm.Add(new SessoesForm()
                        {
                            Id = session.Id,
                            SessionDate = new DateTime(session.SessionDate),
                            StartHour = new TimeSpan(session.StartHour),
                            EndHour = new TimeSpan(session.EndHour),
                            TotalPlaces = session.TotalPlaces,
                            nameShow = session.Show.Name,
                            Estado = (session.Estado == 1) ? "Ativo" : "Desativo",
                            EstadoColor = (session.Estado == 1) ? "Green" : "Red",
                        });
                    }
                    ListSessoes.ItemsSource = sessionsForm;
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

                MessageBox.Show("Ocorreu um erro ao atualizar a sessão", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                MessageBox.Show("Ocorreu um erro ao atualizar a sessão", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch(ArgumentOutOfRangeException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentOutOfRangeException': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro ao atualizar a sessão", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                MessageBox.Show("Ocorreu um erro ao atualizar a sessão", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void Voltar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tickerSessions.Stop();
                tickerSessions.Close();

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
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. \nTentativa: {VoltarTries}", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Voltar_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. Poderá não ter ligação à internet. Com isto foi redirecionado para a página de Login", "Lista de Sessões", MessageBoxButton.OK, MessageBoxImage.Error);

                    Login login = new();

                    login.Show();

                    Close();
                }
                VoltarTries--;
            }
        }
    }

    public class SessoesForm
    {
        public int Id { get; set; }
        public string nameShow { get; set; } = "";
        public DateTime SessionDate { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public Decimal TotalPlaces { get; set; }
        public string Estado { get; set; } = "";
        public string EstadoColor { get; set; } = "";
    }
}
