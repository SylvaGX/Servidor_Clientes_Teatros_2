using Client_User.Models;
using Grpc.Core;
using GRPCProto;
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
            string IPformat = "([1-9]|([1-9][0-9])|([1][0-9][0-9])|([2][0-5][0-5]))\\.([0-9]|([1-9][0-9])|([1][0-9][0-9])|([2][0-5][0-5]))\\.([0-9]|([1-9][0-9])|([1][0-9][0-9])|([2][0-5][0-5]))\\.([1-9]|([1-9][0-9])|([1][0-9][0-9])|([2][0-5][0-5]))";
            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password) && !String.IsNullOrEmpty(IPAddress) && Regex.IsMatch(IPAddress, IPformat))
            {
                var channel = new Channel(IPAddress+":45300", ChannelCredentials.Insecure);
                var client = new LoginClient(new GRPCProto.Login.LoginClient(channel));
                UserLogin userLogin = new()
                {
                    Email = email,
                    Password = password,
                };

                // Send and receive some notes.
                UserConnected userConnected = client.CheckLogin(userLogin).Result;
                Password.Text  = userConnected.Type;

                channel.ShutdownAsync().Wait();
            }
            else {
                //view para dar erro
            }
        }
    }
}
