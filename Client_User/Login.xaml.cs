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
            string password = Password.Text;
            string IPAddress = IP.Text;
            string IPformat = "^(?:[0-9]{1,3}\\.){3}[0-9]{1,3}$";
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password) && !String.IsNullOrEmpty(IPAddress) && Regex.IsMatch(IPAddress, IPformat))
            {
                var channel = new Channel(IPAddress+":45300", ChannelCredentials.Insecure);
                var client = new LoginClient(new gRPCProto.Login.LoginClient(channel));
                UserLogin userLogin = new()
                {
                    Email = email,
                    Password = password,
                };

                // Send and receive some notes.
                UserConnected userConnected = client.CheckLogin(userLogin).Result;

                if (userConnected.Exists())
                {
                    MainWindow.IPAdd = IPAddress;
                    MainWindow mainWindow = new MainWindow(userConnected);
                    mainWindow.Show();
                    this.Close();
                }
                
                channel.ShutdownAsync().Wait();
            }
            else {
                //view para dar erro
            }
        }

        bool hasBeenClicked = false;
        bool hasBeenClicked1 = false;
        bool hasBeenClicked2 = false;

        private void TextBox_Focus(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked)
            { 
                TextBox box = sender as TextBox;
                box.Text = String.Empty;
                hasBeenClicked = true;        
            }
        }
        private void TextBox_Focus1(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked1)
            {
                TextBox box = sender as TextBox;
                box.Text = String.Empty;
                hasBeenClicked1 = true;
            }
        }
        private void TextBox_Focus2(object sender, RoutedEventArgs e)
        {
            if (!hasBeenClicked2)
            {
                TextBox box = sender as TextBox;
                box.Text = String.Empty;
                hasBeenClicked2 = true;
            }
        }
    }
}
