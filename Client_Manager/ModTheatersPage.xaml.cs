using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for ModTeatrosPage.xaml
    /// </summary>
    public partial class ModTeatrosPage : Page
    {
        UserConnected userConnected { get; }
        Action<int> Close { get; }
        TheaterInfo theater { get; }
        public ModTeatrosPage(UserConnected userConnected, TheaterInfo theaterInfo, Action<int> close)
        {
            InitializeComponent();

            this.userConnected = userConnected;
            Close = close;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientL = new LocalizationServiceClient(channel, new LocalizationService.LocalizationServiceClient(channel));

            IEnumerable<LocalizationInfo> localizations = clientL.GetLocalizations(userConnected).Result;

            theater = theaterInfo;

            Id.Text = theaterInfo.Id.ToString();
            var IDTextFormatedText = new FormattedText(
                Id.Text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(Id.FontFamily, Id.FontStyle, Id.FontWeight, Id.FontStretch),
                Id.FontSize,
                Brushes.White,
                new NumberSubstitution(),
                1
            );
            double distId = new Size(IDTextFormatedText.Width, IDTextFormatedText.Height).Width;
            if ((381 - distId) < 0)
            {
                Id.Margin = new Thickness(distId - 381, 40, 0, 0);
            }
            else
            {
                Id.Margin = new Thickness(0, 40, 381 - distId, 0);
            }

            Nome.Text = theaterInfo.Name;
            Endereco.Text = theaterInfo.Address;
            Contacto.Text = theaterInfo.Contact;

            if (localizations.Any())
            {
                ComboBoxItem? selected = null;
                foreach (LocalizationInfo localization in localizations)
                {
                    ComboBoxItem it = new ComboBoxItem()
                    {
                        Uid = localization.Id.ToString(),
                        Content = localization.Name,
                    };

                    if (selected != null && localization.Id == theaterInfo.Localization.Id)
                    {
                        it.IsSelected = true;
                        selected = it;
                    }
                    Localizacao.Items.Add(it);
                }
                Localizacao.SelectedItem = selected;
            }

        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close.Invoke(0);
        }

        private void Submeter_Click(object sender, RoutedEventArgs e)
        {

            string nome = Nome.Text;
            string endereco = Endereco.Text;
            string contacto = Contacto.Text;
            ComboBoxItem? locItem = (Localizacao.SelectedItem as ComboBoxItem);

            if (locItem != null)
            {
                string loc = locItem.Uid;
                if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(endereco) && !string.IsNullOrEmpty(contacto) && !string.IsNullOrEmpty(loc))
                {
                    bool r = int.TryParse(loc, out int locInt);
                    if (r)
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new TheaterServiceClient(channel, new TheaterService.TheaterServiceClient(channel));

                        TheaterInfo theaterInfo = new TheaterInfo()
                        {
                            Id = theater.Id,
                            Name = nome,
                            Address = endereco,
                            Contact = contacto,
                            Estado = theater.Estado,
                            Localization = new LocalizationInfo()
                            {
                                Id = locInt,
                            }
                        };

                        Confirmation confirmation = client.UpdateTheater(theaterInfo).Result;

                        if (confirmation.Exists())
                        {
                            //sucesso
                            MessageBox.Show("Teatro atualizado com sucesso.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close.Invoke(1);
                        }
                        else
                        {
                            //Erro ao inserir o teatro
                            MessageBox.Show("Erro ao atualizar o teatro.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            else
            {
                MessageBox.Show("Insira uma localização.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
