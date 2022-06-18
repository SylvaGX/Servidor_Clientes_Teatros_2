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

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new LocalizationServiceClient(channel, new LocalizationService.LocalizationServiceClient(channel));
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
            string format = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$"; ;
            var emailValidate = Regex.IsMatch(email, format);
            if (emailValidate)
            {
                int idLoc = int.Parse(((ComboBoxItem)idLocalization.SelectedItem).Uid);
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && idLoc > 0)
                {
                    var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                    var client = new RegisterClient(channel, new gRPCProto.Register.RegisterClient(channel));

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
                        MainWindow mainWindow = new MainWindow(userConnected);
                        mainWindow.Show();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Registo Inválido, o email já existe", "Registo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }


                    channel.ShutdownAsync().Wait();
                }
            }
            else
            {
                    MessageBox.Show("Registo Inválido, o formato de email que inseriu é inválido.", "Registo", MessageBoxButton.OK, MessageBoxImage.Error);
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
                TextBox? box = sender as TextBox;
                if(box != null)
                    box.Text = string.Empty;
                hasBeenClicked1 = true;
            }
        }
    }
}
