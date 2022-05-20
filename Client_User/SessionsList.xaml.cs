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
    /// Interaction logic for SessionsList.xaml
    /// </summary>
    public partial class SessionsList : Window
    {
        List<SessionInfoForm> sessionsForms { get; } = new List<SessionInfoForm>();
        IEnumerable<SessionInfo> sessions { get; set; } = null!;
        public SessionsList(ShowInfo show)
        {
            InitializeComponent();

            var channel = new Channel(MainWindow.IPAdd + ":45300", ChannelCredentials.Insecure);
            var client = new SessionServiceClient(new SessionService.SessionServiceClient(channel));

            sessions = client.GetSessions(show).Result;

            if (sessions.Any())
            {
                foreach (SessionInfo session in sessions)
                {
                    sessionsForms.Add(new SessionInfoForm()
                    {
                        Id = session.Id,
                        SessionDate = new DateTime(session.SessionDate),
                        StartHour = new DateTime(session.StartHour),
                        EndHour = new DateTime(session.EndHour),
                        AvaiablePlaces = session.AvaiablePlaces,
                        TotalPlaces = session.TotalPlaces,
                        PercentagePlaces = decimal.Divide(session.AvaiablePlaces, session.TotalPlaces) * 100,
                        color_ULOffset = 2 - decimal.Divide(session.AvaiablePlaces, session.TotalPlaces) / 100,
                    });
                }
                ListaSessoes.ItemsSource = sessionsForms;
            }

            channel.ShutdownAsync().Wait();
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    public class SessionInfoForm
    {
        public int Id { get; set; }
        public DateTime SessionDate { get; set; }
        public DateTime StartHour { get; set; }
        public DateTime EndHour { get; set; }
        public int AvaiablePlaces { get; set; }
        public int TotalPlaces { get; set; }
        public decimal PercentagePlaces { get; set; }
        public decimal color_ULOffset { get; set; }
    }
}
