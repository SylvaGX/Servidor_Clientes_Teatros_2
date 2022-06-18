using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for SessoesList.xaml
    /// </summary>
    public partial class UserList : Window
    {
        UserConnected userConnected { get; }
        int VoltarTries = 5;
        List<UsersForm> usersForms { get; set; } = new List<UsersForm>();
        IEnumerable<UserInfo> users;

        Timer tickerSessions = new Timer();
        public UserList(UserConnected userConnected)
        {
            this.userConnected = userConnected;
            users = new List<UserInfo>();

            try
            {
                InitializeComponent();

                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var clientSessions = new UserServiceClient(channel, new UserService.UserServiceClient(channel));

                users = clientSessions.GetUsers(userConnected).Result;

                if (users.Any())
                {
                    foreach (UserInfo user in users)
                    {
                        usersForms.Add(new UsersForm()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            Local = user.Localization.Name
                        });
                    }
                    UsersList.ItemsSource = usersForms;
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

                MessageBox.Show("Ocorreu um erro", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Error);

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
                UpdateUserList();
            }), null);
        }

        public async void UpdateUserList()
        {
            try
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new UserServiceClient(channel, new UserService.UserServiceClient(channel));

                users = client.GetUsers(userConnected).Result;

                if (users.Any())
                {
                    UsersList.ItemsSource = null;
                    usersForms.Clear();
                    foreach (UserInfo user in users)
                    {
                        usersForms.Add(new UsersForm()
                        {
                            Id = user.Id,
                            Name = user.Name,
                            Email = user.Email,
                            Local = user.Localization.Name
                        });
                    }
                    UsersList.ItemsSource = usersForms;
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

                MessageBox.Show("Ocorreu um erro ao atualizar a lista de users", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                MessageBox.Show("Ocorreu um erro ao atualizar a lista de users", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                MessageBox.Show("Ocorreu um erro ao atualizar a lista de users", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private async void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddUser addUser = new(userConnected);

                addUser.Show();

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

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a página adicionar users.", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Voltar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tickerSessions.Stop();
                tickerSessions.Close();

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

                if (VoltarTries > 0)
                {
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. \nTentativa: {VoltarTries}", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Voltar_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"Ocorreu um erro ao voltar para o Inicio. Poderá não ter ligação à internet. Com isto foi redirecionado para a página de Login", "Lista de Users", MessageBoxButton.OK, MessageBoxImage.Error);

                    Login login = new();

                    login.Show();

                    Close();
                }
                VoltarTries--;
            }
        }
    }

    public class UsersForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Local { get; set; } = "";
    }
}
