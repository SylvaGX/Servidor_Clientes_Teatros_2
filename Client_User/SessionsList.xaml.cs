using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

            var channel = new Channel(App.IPAdd + ":45300", ChannelCredentials.Insecure);
            var client = new SessionServiceClient(new SessionService.SessionServiceClient(channel));

            sessions = client.GetSessions(show).Result;

            if (sessions.Any())
            {
                foreach (SessionInfo session in sessions)
                {
                    sessionsForms.Add(new SessionInfoForm()
                    {
                        Id = session.Id,
                        NameShow = show.Name,
                        TheaterName = show.Theater.Name,
                        TheaterAdress = show.Theater.Address,
                        SessionDate = new DateTime(session.SessionDate),
                        StartHour = new DateTime(session.StartHour),
                        EndHour = new DateTime(session.EndHour),
                        AvaiablePlaces = session.AvaiablePlaces,
                        TotalPlaces = session.TotalPlaces,
                        PercentagePlaces = decimal.Divide(session.AvaiablePlaces, session.TotalPlaces) * 100,
                        OffsetRed = Math.Max(2 - decimal.Divide(session.AvaiablePlaces, session.TotalPlaces), 1),
                        OffsetOrange = Math.Max(1.5m - decimal.Divide(session.AvaiablePlaces, session.TotalPlaces), 0.5m),
                        Price = Convert.ToDecimal(show.Price),
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

        private void Submeter_Click(object sender, RoutedEventArgs e)
        {
            var channel = new Channel(App.IPAdd + ":45300", ChannelCredentials.Insecure);
            var client = new CarServiceClient(new CartService.CartServiceClient(channel));

            Confirmation confirmation;

            foreach (SessionInfoForm sessionForm in sessionsForms)
            {
                if(sessionForm.NumberPlaces > 0)
                {
                    confirmation = client.ReservePlaces(new SessionInfoReserve()
                    {
                        Id = sessionForm.Id,
                        NumberPlaces = sessionForm.NumberPlaces,
                    }).Result;

                    sessionForm.IdNumberPlaces = sessionForm.Id.ToString() + "," + sessionForm.NumberPlaces.ToString();

                    if (confirmation.Exists())
                    {
                        if (confirmation.Id == 1)
                        {
                            App.carrinho.Add(sessionForm);
                        }
                        else
                        {
                            // Guardar numa var para imprimir que já nao tem espaço para os logares reservados
                        }
                    }
                    else
                    {
                        // Erro ao encontrar a sessao na BD
                    }
                }
            }

            channel.ShutdownAsync().Wait();

            Close();
        }

        private static readonly Regex regex = new Regex("^0$|(^[1-9][0-9]*$)");

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox != null && regex.IsMatch(textBox.Text + e.Text))
            {
                SessionInfoForm? sessionInfoForm = sessionsForms.FirstOrDefault(s => s.Id.Equals(Convert.ToInt32(textBox.Tag)));

                if (sessionInfoForm != null && !string.IsNullOrEmpty(textBox.Text + e.Text))
                {
                    int v = Convert.ToInt32(textBox.Text + e.Text);

                    if (v <= (sessionInfoForm.TotalPlaces - sessionInfoForm.AvaiablePlaces))
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled= true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }
    public class SessionInfoForm
    {
        public int Id { get; set; }
        public string NameShow { get; set; } = "";
        public string TheaterName { get; set; } = "";
        public string TheaterAdress { get; set; } = "";
        public DateTime SessionDate { get; set; }
        public DateTime StartHour { get; set; }
        public DateTime EndHour { get; set; }
        public int AvaiablePlaces { get; set; }
        public int TotalPlaces { get; set; }
        public decimal PercentagePlaces { get; set; }
        public decimal OffsetRed { get; set; } = 0.0m;
        public decimal OffsetOrange { get; set; } = 0.0m;
        public decimal Price { get; set; } = 0.0m;
        public int NumberPlaces { get; set; } = 0;
        public string IdNumberPlaces { get; set; } = "";
        
    }
}
