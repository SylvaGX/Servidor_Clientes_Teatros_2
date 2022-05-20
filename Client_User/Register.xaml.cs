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

namespace Client_User
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();

            var channel = new Channel("10.144.10.2:45300", ChannelCredentials.Insecure);
            var client = new LocalizationServiceClient(new LocalizationService.LocalizationServiceClient(channel));
            UserConnected userConnected = new()
            {
                Id = -1,
                Type = "",
            };

            // Send and receive some notes.
            IEnumerable<LocalizationInfo> localizations = client.GetLocalizations(userConnected).Result;

            if (localizations.Any())
            {
                foreach(var localization in localizations)
                {
                    idLocalization.Items.Add(new ComboBoxItem(){
                        Uid = localization.Id.ToString(),
                        Content = localization.Name,
                    });
                }

                idLocalization.SelectedIndex = 0;
            }

            channel.ShutdownAsync().Wait();
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Login login = new();
            login.Show();
            Close();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string email = Email.Text;
            string password = Password.Password;
            int idLoc = int.Parse(((ComboBoxItem)idLocalization.SelectedItem).Uid);
            if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && idLoc > 0)
            {
                var channel = new Channel("10.144.10.2:45300", ChannelCredentials.Insecure);
                var client = new RegisterClient(new gRPCProto.Register.RegisterClient(channel));
                
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
                    MainWindow.IPAdd = "10.144.10.2";
                    MainWindow mainWindow = new MainWindow(userConnected);
                    mainWindow.Show();
                    Close();
                }


                channel.ShutdownAsync().Wait();
            }
        }
    }
}
