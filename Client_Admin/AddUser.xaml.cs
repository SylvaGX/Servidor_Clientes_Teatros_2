using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
    /// Interaction logic for AddAdmin.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        UserConnected userConnected { get; }
        int CancelarTries = 5;
        public AddUser(UserConnected userConnected)
        {
            this.userConnected = userConnected;

            try
            {
                InitializeComponent();

                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new LocalizationServiceClient(channel, new LocalizationService.LocalizationServiceClient(channel));

                // Send and receive some notes.
                IEnumerable<LocalizationInfo> localizations = client.GetLocalizations(userConnected).Result;

                if (localizations.Any())
                {
                    foreach (var localization in localizations)
                    {
                        idLocalization.Items.Add(new ComboBoxItem()
                        {
                            Uid = localization.Id.ToString(),
                            Content = localization.Name,
                        });
                    }

                    idLocalization.SelectedIndex = 0;
                }

                channel.ShutdownAsync().Wait();
            }
            catch (ObjectDisposedException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                logClient.LogError(new LogInfo()
                {
                    Msg = $"'ObjectDisposedException': [{DateTime.Now}] - Error - Erro ao esperar pelo canal fechar. ObjectDisposedException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Adicionar Utilizador", MessageBoxButton.OK, MessageBoxImage.Error);

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
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao obter as localizações. ArgumentNullException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Adicionar Utilizador", MessageBoxButton.OK, MessageBoxImage.Error);

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
                    Msg = $"'Exception': [{DateTime.Now}] - Error. Exception.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }).Wait();

                MessageBox.Show("Ocorreu um erro", "Adicionar Utilizador", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
        }

        private async void Submeter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = Nome.Text;
                string email = Email.Text;
                string password = Pass.Password;
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    string format = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$"; ;
                    var emailValidate = Regex.IsMatch(email, format);

                    if (emailValidate)
                    {
                        int idLoc = int.Parse(((ComboBoxItem)idLocalization.SelectedItem).Uid);
                        if (idLoc > 0)
                        {
                            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                            var client = new RegisterClient(channel, new Register.RegisterClient(channel));

                            UserRegister userRegister = new()
                            {
                                Name = username,
                                Email = email,
                                Password = password,
                                IdLocalization = idLoc,
                            };

                            // Send and receive some notes.
                            UserConnected userConnected = client.RegisterUser(userRegister).Result;

                            if (userConnected.Exists())
                            {
                                UserList userList = new(userConnected);
                                userList.Show();
                                Close();
                            }
                            else
                            {
                                MessageBox.Show("Registo Inválido, o email já existe", "Registo Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            channel.ShutdownAsync().Wait();
                        }
                        else
                        {
                            MessageBox.Show("Erro ao selecionar uma localização", "Registo Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Registo Inválido, o formato de email que inseriu é inválido.", "Registo Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Registo Inválido, insira valores nos campos.", "Registo Manager", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
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

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (ArgumentNullException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (ArgumentException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentException': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (RegexMatchTimeoutException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RegexMatchTimeoutException': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (FormatException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'FormatException': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
            catch (OverflowException ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'OverflowException': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

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

                MessageBox.Show("Ocorreu um erro", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                MainWindow mainWindow = new(userConnected);
                mainWindow.Show();
                Close();
            }
        }

        private async void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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
                    LevelLog = 2
                });

                if (CancelarTries > 0)
                {
                    MessageBox.Show($"Ocorreu um erro ao . \nTentativa: {CancelarTries}", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Cancelar_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"Ocorreu um erro ao cancelar. Poderá não ter ligação à internet. Com isto foi redirecionado para a página de Login", "Adicionar Manager", MessageBoxButton.OK, MessageBoxImage.Error);

                    Login login = new();

                    login.Show();

                    Close();
                }
                CancelarTries--;
            }
        }
    }
}
