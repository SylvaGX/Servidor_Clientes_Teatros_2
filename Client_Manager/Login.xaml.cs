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

namespace Client_Manager
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

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            string email = Email.Text;
            string password = Password.Password;
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password))
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new LoginClient(new gRPCProto.Login.LoginClient(channel));
                UserLogin userLogin = new()
                {
                    Email = email,
                    Password = password,
                    Type = "2",
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
            else
            {
                //view para dar erro
                MessageBox.Show("Insira valores válidos", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
