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
    /// Interaction logic for TeatrosList.xaml
    /// </summary>
    public partial class TeatrosList : Window
    {
        UserConnected userConnected { get; }
        List<TeatrosForm> teatrosForm { get; set; } = new List<TeatrosForm>();
        IEnumerable<TheaterInfo> theaters;

        Timer tickerTheaters = new Timer();
        public TeatrosList(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientTheater = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

            theaters = clientTheater.GetTheaters(userConnected).Result;

            if (theaters.Any())
            {
                foreach (TheaterInfo theater in theaters)
                {
                    teatrosForm.Add(new TeatrosForm()
                    {
                        Id = theater.Id,
                        Name = theater.Name,
                        Loc = theater.Localization.Name,
                        Estado = (theater.Estado == 1) ? "Ativo" : "Desativo",
                        EstadoColor = (theater.Estado == 1) ? "Green" : "Red",
                    });
                }
                ListTeatros.ItemsSource = teatrosForm;
            }

            channel.ShutdownAsync().Wait();

            tickerTheaters.Elapsed += new ElapsedEventHandler(TickerTheaters);
            tickerTheaters.Interval = 5000; // 1000 ms is one second
            tickerTheaters.Start();
        }

        public void TickerTheaters(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateTeatrosList();
            }), null);
        }

        public void UpdateTeatrosList()
        {
            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

            theaters = client.GetTheaters(userConnected).Result;

            if (theaters.Any())
            {
                ListTeatros.ItemsSource = null;
                teatrosForm.Clear();
                foreach (TheaterInfo theater in theaters)
                {
                    teatrosForm.Add(new TeatrosForm()
                    {
                        Id = theater.Id,
                        Name = theater.Name,
                        Loc = theater.Localization.Name,
                        Estado = (theater.Estado == 1) ? "Ativo" : "Desativo",
                        EstadoColor = (theater.Estado == 1) ? "Green" : "Red",
                    });
                }
                ListTeatros.ItemsSource = teatrosForm;
            }

            channel.ShutdownAsync().Wait();
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            tickerTheaters.Stop();
            tickerTheaters.Close();

            MainWindow mainWindow = new MainWindow(userConnected);

            mainWindow.Show();

            Close();
        }
    }
    public class TeatrosForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Loc { get; set; } = "";
        public string Estado { get; set; } = "";
        public string EstadoColor { get; set; } = "";
    }
}
