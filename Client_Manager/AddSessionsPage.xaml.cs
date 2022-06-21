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
    /// Interaction logic for AddSessionsPage.xaml
    /// </summary>
    public partial class AddSessionsPage : Page
    {
        UserConnected userConnected { get; }
        Action<int> Close { get; }
        public AddSessionsPage(UserConnected userConnected, Action<int> close)
        {
            InitializeComponent();

            this.userConnected = userConnected;
            Close = close;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new ShowServiceClient(channel, new ShowService.ShowServiceClient(channel));

            IEnumerable<ShowInfo> shows = client.GetShows(userConnected).Result.Where(s => s.Estado.Equals(1));

            if (shows.Any())
            {
                foreach (ShowInfo show in shows)
                {
                    ComboBoxItem t = new ComboBoxItem()
                    {
                        Uid = show.Id.ToString(),
                        Content = show.Name,
                    };
                    Show.Items.Add(t);
                }
            }
        }

        private void Submeter_Click(object sender, RoutedEventArgs e)
        {
            string sessionDate = SessionDate.Text;
            string startHour = StartHour.Text;
            string endHour = EndHour.Text;
            string totalPlaces = TotalPlaces.Text;
            ComboBoxItem? showItem = (Show.SelectedItem as ComboBoxItem);

            if (showItem != null && !string.IsNullOrEmpty(sessionDate) && !string.IsNullOrEmpty(startHour) && !string.IsNullOrEmpty(endHour) && !string.IsNullOrEmpty(totalPlaces))
            {
                bool r = DateTime.TryParse(sessionDate, out DateTime sessionDateParsed);
                bool r1 = TimeSpan.TryParse(startHour, out TimeSpan startHourParsed);
                bool r2 = TimeSpan.TryParse(endHour, out TimeSpan endHourParsed);
                bool r3 = int.TryParse(totalPlaces, out int totalPlacesParsed);
                bool r4 = int.TryParse(showItem.Uid, out int idParsed);
                if (r && r1 && r2 && r3 && r4)
                {
                    if(sessionDateParsed > DateTime.Now || (sessionDateParsed == DateTime.Now && startHourParsed >= DateTime.Now.AddHours(8).TimeOfDay))
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new SessionServiceClient(channel, new SessionService.SessionServiceClient(channel));

                        SessionInfo sessionInfo = new SessionInfo()
                        {
                            SessionDate = sessionDateParsed.Ticks,
                            StartHour = startHourParsed.Ticks,
                            EndHour = endHourParsed.Ticks,
                            TotalPlaces = totalPlacesParsed,
                            Show = new ShowInfo()
                            {
                                Id = idParsed,
                            }
                        };

                        Confirmation confirmation = client.AddSession(sessionInfo).Result;

                        if (confirmation.Exists())
                        {
                            //sucesso
                            MessageBox.Show("Sessão inserido com sucesso.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close.Invoke(3);
                        }
                        else
                        {
                            //Erro ao inserir o teatro
                            MessageBox.Show("Erro ao inserir a sessão.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Insira uma hora válida.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            Close.Invoke(0);
        }
    }
}
