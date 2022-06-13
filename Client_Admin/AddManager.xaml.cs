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
    public partial class AddManager : Window
    {
        UserConnected userConnected { get; }
        public AddManager(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new LocalizationServiceClient(new LocalizationService.LocalizationServiceClient(channel));

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

        private void Submeter_Click(object sender, RoutedEventArgs e)
        {
            string username = Nome.Text;
            string email = Email.Text;
            string password = Pass.Password;
            if(!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                string format = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$"; ;
                var emailValidate = Regex.IsMatch(email, format);

                if (emailValidate)
                {
                    int idLoc = int.Parse(((ComboBoxItem)idLocalization.SelectedItem).Uid);
                    if (idLoc > 0)
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new RegisterClient(new Register.RegisterClient(channel));

                        ManagerRegister userRegister = new()
                        {
                            Name = username,
                            Email = email,
                            Password = password,
                            IdLocalization = idLoc,
                        };

                        // Send and receive some notes.
                        UserConnected userConnected = client.RegisterManager(userRegister).Result;

                        if (userConnected.Exists())
                        {
                            MainWindow mainWindow = new(userConnected);
                            mainWindow.Show();
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

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new(userConnected);

            mainWindow.Show();

            Close();
        }

    }
}
