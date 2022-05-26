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
    /// Interaction logic for AddShowsPage.xaml
    /// </summary>
    public partial class AddShowsPage : Page
    {
        UserConnected userConnected { get; }
        Action<int> Close { get; }
        public AddShowsPage(UserConnected userConnected, Action<int> close)
        {
            InitializeComponent();

            this.userConnected = userConnected;
            Close = close;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

            IEnumerable<TheaterInfo> theaters = client.GetTheaters(userConnected).Result;

            if (theaters.Any())
            {
                foreach (TheaterInfo theater in theaters)
                {
                    ComboBoxItem t = new ComboBoxItem()
                    {
                        Uid = theater.Id.ToString(),
                        Content = theater.Name,
                    };
                    Theater.Items.Add(t);
                }
            }

            channel.ShutdownAsync().Wait();
        }

        private void Submeter_Click(object sender, RoutedEventArgs e)
        {
            string nome = Nome.Text;
            string sinopse = Sinopse.Text;
            string startDate = StartDate.Text;
            string endDate = EndDate.Text;
            string price = Price.Text;
            ComboBoxItem? theaterItem = (Theater.SelectedItem as ComboBoxItem);

            if (theaterItem != null)
            {
                string loc = theaterItem.Uid;
                if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(sinopse) && !string.IsNullOrEmpty(startDate) 
                    && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(price))
                {
                    bool r = int.TryParse(loc, out int locInt);
                    bool r1 = DateTime.TryParse(startDate, out DateTime startDateParsed);
                    bool r2 = DateTime.TryParse(endDate, out DateTime endDateParsed);
                    bool r3 = decimal.TryParse(price, out decimal priceParsed);
                    if (r && r1 && r2 && r3)
                    {
                        if (startDateParsed > DateTime.Now)
                        {
                            if (startDateParsed < endDateParsed) {
                                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                                var client = new ShowServiceClient(new ShowService.ShowServiceClient(channel));

                                ShowInfo showInfo = new ShowInfo()
                                {
                                    Name = nome,
                                    Sinopse = sinopse,
                                    StartDate = startDateParsed.Ticks,
                                    EndDate = endDateParsed.Ticks,
                                    Price = decimal.ToDouble(priceParsed),
                                    Theater = new TheaterInfo()
                                    {
                                        Id = locInt,
                                    }
                                };

                                Confirmation confirmation = client.AddShow(showInfo).Result;

                                if (confirmation.Exists())
                                {
                                    //sucesso
                                    MessageBox.Show("Espetáculo inserido com sucesso.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Close.Invoke(2);
                                }
                                else
                                {
                                    //Erro ao inserir o teatro
                                    MessageBox.Show("Erro ao inserir o espetáculo.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                            else
                            {
                                MessageBox.Show("A Start Date deve ser maior que a End Date.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("A Start Date deve ser maior que a data de hoje.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
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
