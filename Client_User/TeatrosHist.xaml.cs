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
    /// Interaction logic for TeatrosHist.xaml
    /// </summary>
    public partial class TeatrosHist : Window
    {
        public UserConnected userConnected { get; set; }
        List<PurchaseInfoForm> purchaseInfos = new List<PurchaseInfoForm>();
        IEnumerable<PurchaseInfo> purchases { get; set; }
        public TeatrosHist(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new CompraServiceClient(new CompraService.CompraServiceClient(channel));

            purchases = client.HistoryUser(userConnected).Result;

            foreach (PurchaseInfo purchase in purchases)
            {
                purchaseInfos.Add(new PurchaseInfoForm()
                {
                    Id = purchase.Id,
                    Reference = purchase.Reference,
                    DatePurchase = new DateTime(purchase.DatePurchase),
                    NumberPlaces = purchase.CompraLugares,
                    IdSession = purchase.Session.Id,
                    SessionSessionDate = new DateTime(purchase.Session.SessionDate),
                    SessionStartHour = new TimeSpan(purchase.Session.StartHour),
                    SessionEndHour = new TimeSpan(purchase.Session.EndHour),
                    SessionAvaiablePlaces = purchase.Session.AvaiablePlaces,
                    SessionTotalPlaces = purchase.Session.TotalPlaces,
                    IdShow = purchase.Session.Show.Id,
                    ShowName = purchase.Session.Show.Name,
                    ShowSinopse = purchase.Session.Show.Sinopse,
                    ShowStartDate = new DateTime(purchase.Session.Show.StartDate),
                    ShowEndDate = new DateTime(purchase.Session.Show.EndDate),
                    ShowPrice = Convert.ToDecimal(purchase.Session.Show.Price),
                    IdTheater = purchase.Session.Show.Theater.Id,
                    TheaterName = purchase.Session.Show.Theater.Name,
                    TheaterAdress = purchase.Session.Show.Theater.Address,
                    TheaterContact = purchase.Session.Show.Theater.Contact,
                    IdLocalization = purchase.Session.Show.Theater.Localization.Id,
                    LocalizationName = purchase.Session.Show.Theater.Localization.Name,
                    LocalizationLat = Convert.ToDecimal(purchase.Session.Show.Theater.Localization.Lat),
                    LocalizationLongi = Convert.ToDecimal(purchase.Session.Show.Theater.Localization.Longi),
                });
            }

            HistoricoCompras.ItemsSource = purchaseInfos;

            channel.ShutdownAsync().Wait();

        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(userConnected);

            mainWindow.Show();

            Close();
        }
    }

    public class PurchaseInfoForm
    {
        public int Id { get; set; }
        public string Reference { get; set; } = "";
        public DateTime DatePurchase { get; set; }
        public int NumberPlaces { get; set; } = 0;

        public int IdSession { get; set; }
        public DateTime SessionSessionDate { get; set; }
        public TimeSpan SessionStartHour { get; set; }
        public TimeSpan SessionEndHour { get; set; }
        public int SessionAvaiablePlaces { get; set; }
        public int SessionTotalPlaces { get; set; }

        public int IdShow { get; set; }
        public string ShowName { get; set; } = "";
        public string ShowSinopse { get; set; } = "";
        public DateTime ShowStartDate { get; set; }
        public DateTime ShowEndDate { get; set; }
        public decimal ShowPrice { get; set; } = 0.0m;

        public int IdTheater { get; set; }
        public string TheaterName { get; set; } = "";
        public string TheaterAdress { get; set; } = "";
        public string TheaterContact { get; set; } = "";


        public int IdLocalization { get; set; }
        public string LocalizationName { get; set; } = "";
        public decimal LocalizationLat { get; set; } = 0.0m;
        public decimal LocalizationLongi { get; set; } = 0.0m;
    }
}
