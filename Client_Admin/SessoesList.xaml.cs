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
    /// Interaction logic for SessoesList.xaml
    /// </summary>
    public partial class SessoesList : Window
    {
        UserConnected userConnected { get; }
        List<SessoesForm> sessionsForm { get; set; } = new List<SessoesForm>();
        IEnumerable<SessionInfo> sessions;

        Timer tickerSessions = new Timer();
        public SessoesList(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientSessions = new SessionServiceClient(new SessionService.SessionServiceClient(channel));

            sessions = clientSessions.GetAllSessions(userConnected).Result;

            if (sessions.Any())
            {
                foreach (SessionInfo session in sessions)
                {
                    sessionsForm.Add(new SessoesForm()
                    {
                        Id = session.Id,
                        SessionDate = new DateTime(session.SessionDate),
                        StartHour = new TimeSpan(session.StartHour),
                        EndHour = new TimeSpan(session.EndHour),
                        TotalPlaces = session.TotalPlaces,
                        nameShow = session.Show.Name,
                        Estado = (session.Estado == 1) ? "Ativo" : "Desativo",
                        EstadoColor = (session.Estado == 1) ? "Green" : "Red",
                    });
                }
                ListSessoes.ItemsSource = sessionsForm;
            }

            channel.ShutdownAsync().Wait();

            tickerSessions.Elapsed += new ElapsedEventHandler(TickerSessions);
            tickerSessions.Interval = 5000; // 1000 ms is one second
            tickerSessions.Start();
        }

        public void TickerSessions(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateSessionsList();
            }), null);
        }

        public void UpdateSessionsList()
        {
            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new SessionServiceClient(new SessionService.SessionServiceClient(channel));

            sessions = client.GetAllSessions(userConnected).Result;

            if (sessions.Any())
            {
                ListSessoes.ItemsSource = null;
                sessionsForm.Clear();
                foreach (SessionInfo session in sessions)
                {
                    sessionsForm.Add(new SessoesForm()
                    {
                        Id = session.Id,
                        SessionDate = new DateTime(session.SessionDate),
                        StartHour = new TimeSpan(session.StartHour),
                        EndHour = new TimeSpan(session.EndHour),
                        TotalPlaces = session.TotalPlaces,
                        nameShow = session.Show.Name,
                        Estado = (session.Estado == 1) ? "Ativo" : "Desativo",
                        EstadoColor = (session.Estado == 1) ? "Green" : "Red",
                    });
                }
                ListSessoes.ItemsSource = sessionsForm;
            }

            channel.ShutdownAsync().Wait();
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            tickerSessions.Stop();
            tickerSessions.Close();

            MainWindow mainWindow = new MainWindow(userConnected);

            mainWindow.Show();

            Close();
        }
    }

    public class SessoesForm
    {
        public int Id { get; set; }
        public string nameShow { get; set; } = "";
        public DateTime SessionDate { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public Decimal TotalPlaces { get; set; }
        public string Estado { get; set; } = "";
        public string EstadoColor { get; set; } = "";
    }
}
