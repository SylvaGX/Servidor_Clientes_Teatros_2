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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserConnected userConnected { get; }
        List<TeatrosForm> teatrosForm { get; set; } = new List<TeatrosForm>();
        IEnumerable<TheaterInfo> theaters;
        List<EspetaculosForm> showsForm { get; set; } = new List<EspetaculosForm>();
        IEnumerable<ShowInfo> shows;
        List<SessoesForm> sessionsForm { get; set; } = new List<SessoesForm>();
        IEnumerable<SessionInfo> sessions;
        public MainWindow()
        {
            InitializeComponent();

            userConnected = new UserConnected()
            {
                Id = 2,
                Type = "2",
            };

            var channel = new Channel(App.IPAdd + ":45300", ChannelCredentials.Insecure);
            var clientTheater = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));
            var clientShow = new ShowServiceClient(new ShowService.ShowServiceClient(channel));
            var clientSessions = new SessionServiceClient(new SessionService.SessionServiceClient(channel));

            theaters = clientTheater.GetTheaters(userConnected).Result;

            if (theaters.Any())
            {
                foreach (TheaterInfo theater in theaters)
                {
                    teatrosForm.Add(new TeatrosForm()
                    {
                        Id = theater.Id,
                        Name = theater.Name,
                        Estado = (theater.Estado == 1) ? true : false,
                    });
                }
                ListTeatro.ItemsSource = teatrosForm;
            }
            
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
                        Estado = (show.Estado == 1) ? true : false,
                    });
                }
                ListEspetaculos.ItemsSource = showsForm;
            }
            
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
                        IdShow = session.Show.Id,
                        Estado = (session.Estado == 1) ? true : false,
                    });
                }
                ListSessoes.ItemsSource = sessionsForm;
            }

            channel.ShutdownAsync().Wait();

        }
    }

    public class TeatrosForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool Estado { get; set; }
    }

    public class EspetaculosForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Decimal Price { get; set; }
        public bool Estado { get; set; }
    }

    public class SessoesForm
    {
        public int Id { get; set; }
        public int IdShow { get; set; }
        public DateTime SessionDate { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public Decimal TotalPlaces { get; set; }
        public bool Estado { get; set; }
    }
}
