using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        bool hasBeenClicked = false;
        bool hasBeenClicked1 = false;
        bool hasBeenClicked2 = false;

        private void TextBox_Focus(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked)
            {
                TextBox? box = sender as TextBox;
                if (box != null)
                    box.Text = string.Empty;
                hasBeenClicked = true;
            }
        }
        private void TextBox_Focus1(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked1)
            {
                PasswordBox? box = sender as PasswordBox;
                if (box != null)
                    box.Password = string.Empty;
                hasBeenClicked1 = true;
            }
        }
        private void TextBox_Focus2(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked2)
            {
                TextBox? box = sender as TextBox;
                if (box != null)
                    box.Text = string.Empty;
                hasBeenClicked2 = true;
            }
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = Email.Text;
                string pass = Password.Password;

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(pass))
                {
                    var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                    var client = new LoginClient(channel, new gRPCProto.Login.LoginClient(channel));
                    UserLogin userLogin = new()
                    {
                        Email = email,
                        Password = pass,
                        Type = "3",
                    };

                    // Send and receive some notes.
                    UserConnected userConnected = client.CheckLogin(userLogin).Result;

                    if (userConnected.Exists())
                    {
                        if (userConnected.Id == -2)
                        {
                            MessageBox.Show("Erro ao Iniciar sessão. Utilizador ou Password Incorretos.", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            MainWindow mainWindow = new MainWindow(userConnected);
                            mainWindow.Show();
                            Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erro ao Iniciar sessão. Por favor contactar a entidade", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    channel.ShutdownAsync().Wait();
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

                MessageBox.Show("Ocorreu um erro ao fazer login", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                MessageBox.Show("Ocorreu um erro ao fazer login", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                MessageBox.Show("Ocorreu um erro ao fazer login", "Login", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
