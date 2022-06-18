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
    /// Interaction logic for ModShowsPage.xaml
    /// </summary>
    public partial class ModShowsPage : Page
    {
        UserConnected userConnected { get; }
        Action<int> Close { get; }
        ShowInfo show { get; }
        public ModShowsPage(UserConnected userConnected, ShowInfo showInfo, Action<int> close)
        {
            InitializeComponent();

            this.userConnected = userConnected;
            Close = close;

            show = showInfo;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientL = new TheaterServiceClient(channel, new TheaterService.TheaterServiceClient(channel));

            IEnumerable<TheaterInfo> theaters = clientL.GetTheaters(userConnected).Result;

            ID.Text = showInfo.Id.ToString();
            Nome.Text = showInfo.Name;
            Sinopse.Text = showInfo.Sinopse;
            //Dps se a data for menor que a data atual não pode atualizar, aplicar isto no c# e xaml
            StartDate.Text = new DateTime(showInfo.StartDate).ToShortDateString();
            if(showInfo.StartDate < DateTime.Now.Date.Ticks)
                StartDate.Focusable = false;
            EndDate.Text = new DateTime(showInfo.EndDate).ToShortDateString();
            Price.Text = Convert.ToDecimal(showInfo.Price).ToString("0.00");
            Theater.Text = showInfo.Theater.Name;

            var IDTextFormatedText = new FormattedText(
                            ID.Text,
                            CultureInfo.CurrentCulture,
                            FlowDirection.LeftToRight,
                            new Typeface(ID.FontFamily, ID.FontStyle, ID.FontWeight, ID.FontStretch),
                            ID.FontSize,
                            Brushes.White,
                            new NumberSubstitution(),
                            1
                        );
            double distID = new Size(IDTextFormatedText.Width, IDTextFormatedText.Height).Width;
            if ((381 - distID) < 0)
            {
                ID.Margin = new Thickness(distID - 381, 30, 0, 0);
            }
            else
            {
                ID.Margin = new Thickness(0, 30, 381 - distID, 0);
            }

            var TheaterValFormatedText = new FormattedText(
                        Theater.Text,
                        CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface(Theater.FontFamily, Theater.FontStyle, Theater.FontWeight, Theater.FontStretch),
                        Theater.FontSize,
                        Brushes.White,
                        new NumberSubstitution(),
                        1);
            double distTheater = new Size(TheaterValFormatedText.Width, TheaterValFormatedText.Height).Width;
            if ((381 - distTheater) < 0)
            {
                Theater.Margin = new Thickness(distTheater - 381, 310, 0, 0);
            }
            else
            {
                Theater.Margin = new Thickness(0, 310, 381 - distTheater, 0);
            }
        }

        private void Submeter_Click(object sender, RoutedEventArgs e)
        {
            string nome = Nome.Text;
            string sinopse = Sinopse.Text;
            string startDate = StartDate.Text;
            string endDate = EndDate.Text;
            string price = Price.Text;

            if (!string.IsNullOrEmpty(nome) && !string.IsNullOrEmpty(sinopse) && !string.IsNullOrEmpty(startDate)
                && !string.IsNullOrEmpty(endDate) && !string.IsNullOrEmpty(price))
            {
                bool r1 = DateTime.TryParse(startDate, out DateTime startDateParsed);
                bool r2 = DateTime.TryParse(endDate, out DateTime endDateParsed);
                bool r3 = decimal.TryParse(price, out decimal priceParsed);
                if (r1 && r2 && r3)
                {
                    if (startDateParsed < endDateParsed)
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new ShowServiceClient(channel, new ShowService.ShowServiceClient(channel));

                        ShowInfo showInfo = new ShowInfo()
                        {
                            Id = show.Id,
                            Name = nome,
                            Sinopse = sinopse,
                            StartDate = startDateParsed.Ticks,
                            EndDate = endDateParsed.Ticks,
                            Price = decimal.ToDouble(priceParsed),
                            Estado = show.Estado,
                            Theater = new TheaterInfo()
                            {
                                Id = show.Theater.Id,
                            }
                        };

                        Confirmation confirmation = client.UpdateShow(showInfo).Result;

                        if (confirmation.Exists())
                        {
                            //sucesso
                            MessageBox.Show("Espetáculo atualizado com sucesso.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Information);
                            Close.Invoke(2);
                        }
                        else
                        {
                            //Erro ao inserir o teatro
                            MessageBox.Show("Erro ao atualizar o espetáculo.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("A Start Date deve ser maior que a End Date.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
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
