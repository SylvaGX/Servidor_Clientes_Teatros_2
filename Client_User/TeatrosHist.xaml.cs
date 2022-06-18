using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public partial class TeatrosHist : Window, INotifyPropertyChanged
    {
        public UserConnected userConnected { get; set; }
        List<PurchaseInfoForm> purchaseInfos = new List<PurchaseInfoForm>();
        IEnumerable<PurchaseInfo> purchases { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        Visibility popUpCompra = Visibility.Hidden;
        public Visibility PopUpCompra
        {
            get { return popUpCompra; }
            set
            {
                popUpCompra = value;
                // Call OnPropertyChanged whenever the property is updated
                OnPropertyChanged();
            }
        }
        public TeatrosHist(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new CompraServiceClient(channel, new CompraService.CompraServiceClient(channel));

            purchases = client.HistoryUser(userConnected).Result;

            foreach (PurchaseInfo purchase in purchases)
            {
                purchaseInfos.Add(new PurchaseInfoForm()
                {
                    Id = purchase.Id,
                    Reference = purchase.Reference.Ref,
                    IdReference = purchase.Reference.Id,
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
                    Estado = purchase.Estado,
                    EstadoPag = (purchase.Estado == 1) ? "Hidden" : (purchase.Estado == 2) ? "Visible" : "Hidden",
                    EstadoOK = (purchase.Estado == 1) ? "Visible" : (purchase.Estado == 2) ? "Hidden" : "Visible",
                    EstadoLabel = (purchase.Estado == 1) ? "Pago" : (purchase.Estado == 2) ? "" :"Cancelado",
                    EstadoColor = (purchase.Estado == 1) ? "Green" : (purchase.Estado == 2) ? "Yellow" : "Red",
                    TrashVis = (purchase.Estado == 1) ? "Hidden" : (purchase.Estado == 2) ? "Visible" : "Hidden"
                });

                var p = purchaseInfos.Last();
                p.IdArr = purchaseInfos.LastIndexOf(p);
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

        private async void CancelarBilhete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button button = (Button)sender;
                if (button != null)
                {
                    int id = Convert.ToInt32(button.Tag.ToString());

                    var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                    var client = new CompraServiceClient(channel, new CompraService.CompraServiceClient(channel));

                    PurchaseInfo purchaseInfo2 = new PurchaseInfo()
                    {
                        Id = id,
                    };

                    PurchaseInfo purchaseInfo = client.GetPurchase(purchaseInfo2).Result;

                    Confirmation confirmation = client.CancelPurchase(purchaseInfo).Result;

                    if (confirmation.Exists())
                    {
                        UpdateComprasList();
                    }
                    else
                    {
                        // Erro ao encontrar a sessao na BD
                        MessageBox.Show("Ocorreu um erro. Iremos resolver o problema.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao cancelar o bilhete.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
            }
        }

        public void ClosePopup()
        {
            PopUpCompra = Visibility.Hidden;
            PopGrid.Children.Clear();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateComprasList();
            }), null);
        }

        public void UpdateComprasList()
        {

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new CompraServiceClient(channel, new CompraService.CompraServiceClient(channel));

            purchases = client.HistoryUser(userConnected).Result;

            if (purchases.Any())
            {

                HistoricoCompras.ItemsSource = null;
                purchaseInfos.Clear();
                foreach (PurchaseInfo purchase in purchases)
                {
                    purchaseInfos.Add(new PurchaseInfoForm()
                    {
                        Id = purchase.Id,
                        Reference = purchase.Reference.Ref,
                        IdReference = purchase.Reference.Id,
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
                        Estado = purchase.Estado,
                        EstadoPag = (purchase.Estado == 1) ? "Hidden" : (purchase.Estado == 2) ? "Visible" : "Hidden",
                        EstadoOK = (purchase.Estado == 1) ? "Visible" : (purchase.Estado == 2) ? "Hidden" : "Visible",
                        EstadoLabel = (purchase.Estado == 1) ? "Pago" : (purchase.Estado == 2) ? "" : "Cancelado",
                        EstadoColor = (purchase.Estado == 1) ? "Green" : (purchase.Estado == 2) ? "Yellow" : "Red",
                        TrashVis = (purchase.Estado == 1) ? "Hidden" : (purchase.Estado == 2) ? "Visible" : "Hidden"
                    });

                    var p = purchaseInfos.Last();
                    p.IdArr = purchaseInfos.LastIndexOf(p);
                }

                HistoricoCompras.ItemsSource = purchaseInfos;
            }

            channel.ShutdownAsync().Wait();
        }

        private async void HistoricoCompras_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button? button = sender as Button;

                if (button != null)
                {

                    int id = Convert.ToInt32(button.Tag);

                    PurchaseInfoForm ele = purchaseInfos.ElementAt(id);

                    var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                    var client = new CompraServiceClient(channel, new CompraService.CompraServiceClient(channel));

                    var sessions = purchaseInfos.Where(p => p.IdReference.Equals(ele.IdReference) && p.Estado.Equals(2));

                    double pre = Convert.ToDouble(sessions.Sum(s => s.NumberPlaces * s.ShowPrice));

                    RefCompra refCompra = new RefCompra()
                    {
                        Id = ele.IdReference,
                        Reference = ele.Reference,
                        PrecoTotal = pre
                    };

                    if (refCompra.Exists())
                    {
                        if (refCompra.Id > 0)
                        {
                            PopUpCompra = Visibility.Visible;

                            ReferenciasPage referenciasPage = new ReferenciasPage(refCompra, userConnected, ClosePopup);

                            PopGrid.Children.Add(new Frame()
                            {
                                Content = referenciasPage,
                            });

                        }
                        else
                        {
                            // Error don't have money
                            MessageBox.Show("Você não tem dinheiro para a compra que está a efetuar!", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        // Erro ao encontrar a sessao na BD
                        MessageBox.Show("Ocorreu um erro. Iremos resolver o problema.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    channel.ShutdownAsync().Wait();
                }
                else
                {
                    // Erro
                    MessageBox.Show("Ocorreu um erro. Iremos resolver o problema.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch(Exception ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as compras do utilizador.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
            }
            
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class PurchaseInfoForm
    {

        public int IdArr { get; set; }
        public int Id { get; set; }
        public string Reference { get; set; } = "";
        public int IdReference { get; set; }
        public DateTime DatePurchase { get; set; }
        public int NumberPlaces { get; set; } = 0;
        public int Estado { get; set; } = 0;
        public string EstadoPag { get; set; } = "";
        public string EstadoOK { get; set; } = "";
        public string EstadoLabel { get; set; } = "";
        public string EstadoColor { get; set; } = "";
        public string TrashVis { get; set; } = "";

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
