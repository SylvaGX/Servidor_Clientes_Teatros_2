using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client_User
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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register();
            register.Show();
            this.Close();
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            string email = Email.Text;
            string password = Password.Password;
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password))
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new LoginClient(channel, new gRPCProto.Login.LoginClient(channel));
                UserLogin userLogin = new()
                {
                    Email = email,
                    Password = password,
                    Type = "1",
                };

                // Send and receive some notes.
                UserConnected userConnected = client.CheckLogin(userLogin).Result;

                if (userConnected.Exists())
                {
                    if(userConnected.Id == -2)
                    {
                        MessageBox.Show("Erro ao Iniciar sessão. Utilizador ou Password Incorretos.", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MainWindow mainWindow = new MainWindow(userConnected);
                        mainWindow.Show();
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Utilizador ou Password Incorretos.", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
                channel.ShutdownAsync().Wait();
            }
            else {
                MessageBox.Show("Insira valores válidos.", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
                if(box != null)
                    box.Password = string.Empty;
                hasBeenClicked1 = true;
            }
        }
      
    }
}
