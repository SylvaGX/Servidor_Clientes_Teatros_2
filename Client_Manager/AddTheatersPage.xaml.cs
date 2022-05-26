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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client_Manager
{
    /// <summary>
    /// Interaction logic for AddTeatrosPage.xaml
    /// </summary>
    public partial class AddTeatrosPage : Page
    {
        UserConnected userConnected { get; }
        Action<int> Close { get; }
        public AddTeatrosPage(UserConnected userConnected, Action<int> close)
        {
            InitializeComponent();

            this.userConnected = userConnected;
            Close = close;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientL = new LocalizationServiceClient(new LocalizationService.LocalizationServiceClient(channel));

            IEnumerable<LocalizationInfo> localizations = clientL.GetLocalizations(userConnected).Result;

            if (localizations.Any())
            {
                foreach (LocalizationInfo localization in localizations)
                {
                    ComboBoxItem it = new ComboBoxItem()
                    {
                        Uid = localization.Id.ToString(),
                        Content = localization.Name,
                    };
                    Localizacao.Items.Add(it);
                }
            }

            channel.ShutdownAsync().Wait();
        }

        private void Submeter_Click(object sender, RoutedEventArgs e)
        {
            string nome = Nome.Text;
            string endereco = Endereco.Text;
            string contacto = Contacto.Text;
            ComboBoxItem? locItem = (Localizacao.SelectedItem as ComboBoxItem);

            if(locItem != null)
            {
                string loc = locItem.Uid;
                if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(endereco) && !string.IsNullOrEmpty(contacto) && !string.IsNullOrEmpty(loc))
                {
                    bool r = int.TryParse(loc, out int locInt);
                    if (r)
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

                        TheaterInfo theaterInfo = new TheaterInfo()
                        {
                            Name = nome,
                            Address = endereco,
                            Contact = contacto,
                            Localization = new LocalizationInfo()
                            {
                                Id = locInt,
                            }
                        };

                        Confirmation confirmation = client.AddTheater(theaterInfo).Result;

                        if (confirmation.Exists())
                        {
                            //sucesso
                            MessageBox.Show("Teatro inserido com sucesso.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close.Invoke(1);
                        }
                        else
                        {
                            //Erro ao inserir o teatro
                            MessageBox.Show("Erro ao inserir o teatro.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erro ao converter uma valor.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Insira valores válidos.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close.Invoke(0);
        }
    }
}
