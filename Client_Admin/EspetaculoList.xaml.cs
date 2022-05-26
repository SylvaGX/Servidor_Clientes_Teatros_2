using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
    /// Interaction logic for EspetaculoList.xaml
    /// </summary>
    public partial class EspetaculoList : Window
    {
        UserConnected userConnected { get; }
        List<EspetaculosForm> showsForm { get; set; } = new List<EspetaculosForm>();
        IEnumerable<ShowInfo> shows;

        Timer tickerShows = new Timer();
        public EspetaculoList(UserConnected userConnected)
        {
            InitializeComponent();
            
            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientShow = new ShowServiceClient(new ShowService.ShowServiceClient(channel));

            shows = clientShow.GetShows(userConnected).Result;

            if (shows.Any())
            {
                foreach (ShowInfo show in shows)
                {
                    showsForm.Add(new EspetaculosForm()
                    {
                        Id = show.Id,
                        Name = show.Name,
                        StartDate = new DateTime(show.StartDate),
                        EndDate = new DateTime(show.EndDate),
                        Price = Convert.ToDecimal(show.Price),
                        Estado = (show.Estado == 1) ? "Ativo" : "Desativo",
                        EstadoColor = (show.Estado == 1) ? "Green" : "Red",
                    });
                }
                ListEspetaculos.ItemsSource = showsForm;
            }

            channel.ShutdownAsync().Wait();

            tickerShows.Elapsed += new ElapsedEventHandler(TickerShows);
            tickerShows.Interval = 5000; // 1000 ms is one second
            tickerShows.Start();
        }

        public void TickerShows(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateShowsList();
            }), null);
        }

        public void UpdateShowsList()
        {
            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new ShowServiceClient(new ShowService.ShowServiceClient(channel));

            shows = client.GetShows(userConnected).Result;

            if (shows.Any())
            {
                ListEspetaculos.ItemsSource = null;
                showsForm.Clear();
                foreach (ShowInfo show in shows)
                {
                    showsForm.Add(new EspetaculosForm()
                    {
                        Id = show.Id,
                        Name = show.Name,
                        StartDate = new DateTime(show.StartDate),
                        EndDate = new DateTime(show.EndDate),
                        Price = Convert.ToDecimal(show.Price),
                        Estado = (show.Estado == 1) ? "Ativo" : "Desativo",
                        EstadoColor = (show.Estado == 1) ? "Green" : "Red",
                    });
                }
                ListEspetaculos.ItemsSource = showsForm;
            }

            channel.ShutdownAsync().Wait();
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            tickerShows.Stop();
            tickerShows.Close();

            MainWindow mainWindow = new MainWindow(userConnected);

            mainWindow.Show();

            Close();
        }
    }

    public class EspetaculosForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal Price { get; set; }
        public string Estado { get; set; } = "";
        public string EstadoColor { get; set; } = "";
    }
}
