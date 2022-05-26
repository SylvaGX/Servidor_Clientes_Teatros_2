using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Client_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        UserConnected userConnected { get; }
        List<TeatrosForm> teatrosForm { get; set; } = new List<TeatrosForm>();
        IEnumerable<TheaterInfo> theaters;
        List<EspetaculosForm> showsForm { get; set; } = new List<EspetaculosForm>();
        IEnumerable<ShowInfo> shows;
        List<SessoesForm> sessionsForm { get; set; } = new List<SessoesForm>();
        IEnumerable<SessionInfo> sessions;
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
        public MainWindow(UserConnected userConnected)
        {
            InitializeComponent();

            DataContext = this;

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientTheater = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));
            var clientShow = new ShowServiceClient(new ShowService.ShowServiceClient(channel));
            var clientSessions = new SessionServiceClient(new SessionService.SessionServiceClient(channel));

            theaters = clientTheater.GetTheaters(userConnected).Result;

            PopUpCompra = Visibility.Hidden;

            if (theaters.Any())
            {
                foreach (TheaterInfo theater in theaters)
                {
                    teatrosForm.Add(new TeatrosForm()
                    {
                        Id = theater.Id,
                        Name = theater.Name,
                        Loc = theater.Localization.Name,
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
                        nameShow = session.Show.Name,
                        Estado = (session.Estado == 1) ? true : false,
                    });
                }
                ListSessoes.ItemsSource = sessionsForm;
            }

            channel.ShutdownAsync().Wait();

            var tickerTheaters = new System.Timers.Timer();
            tickerTheaters.Elapsed += new ElapsedEventHandler(TickerTheaters);
            tickerTheaters.Interval = 5000; // 1000 ms is one second
            tickerTheaters.Start();

            var tickerShows = new System.Timers.Timer();
            tickerShows.Elapsed += new ElapsedEventHandler(TickerTheaters);
            tickerShows.Interval = 5000; // 1000 ms is one second
            tickerShows.Start();

            var tickerSessions = new System.Timers.Timer();
            tickerSessions.Elapsed += new ElapsedEventHandler(TickerTheaters);
            tickerSessions.Interval = 5000; // 1000 ms is one second
            tickerSessions.Start();
        }

        public void TickerTheaters(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateTeatrosList();
            }), null);
        }

        public void TickerShows(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateShowsList();
            }), null);
        }

        public void TickerSessions(object? source, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                // - Change your UI information here
                UpdateSessionsList();
            }), null);
        }

        public void UpdateTeatrosList()
        {
            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

            theaters = client.GetTheaters(userConnected).Result;

            if (theaters.Any())
            {
                ListTeatro.ItemsSource = null;
                teatrosForm.Clear();
                foreach (TheaterInfo theater in theaters)
                {
                    teatrosForm.Add(new TeatrosForm()
                    {
                        Id = theater.Id,
                        Name = theater.Name,
                        Loc = theater.Localization.Name,
                        Estado = (theater.Estado == 1) ? true : false,
                    });
                }
                ListTeatro.ItemsSource = teatrosForm;
            }

            channel.ShutdownAsync().Wait();
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
                        Estado = (show.Estado == 1) ? true : false,
                    });
                }
                ListEspetaculos.ItemsSource = showsForm;
            }

            channel.ShutdownAsync().Wait();
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
                        Estado = (session.Estado == 1) ? true : false,
                    });
                }
                ListSessoes.ItemsSource = sessionsForm;
            }

            channel.ShutdownAsync().Wait();
        }

        public void ClosePopup(int UpdateLists = 0)
        {
            switch (UpdateLists)
            {
                case 1:
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // - Change your UI information here
                            UpdateTeatrosList();
                        }), null);
                        break;
                    }
                case 2:
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // - Change your UI information here
                            UpdateShowsList();
                        }), null);
                        break;
                    }
                case 3:
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // - Change your UI information here
                            UpdateSessionsList();
                        }), null);
                        break;
                    }
            }
            PopUpCompra = Visibility.Hidden;
            PopGrid.Children.Clear();
        }

        private void ChangeEstadoTeatro_Click(object sender, RoutedEventArgs e)
        {

            CheckBox? checkBox = (CheckBox)sender;

            if (checkBox != null)
            {
                string? id = checkBox.Tag.ToString();
                bool? val = checkBox.IsChecked;

                if (id != null && val != null)
                {

                    bool r1 = int.TryParse(id, out int idInt);

                    if (r1)
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

                        Confirmation confirmation = client.ChangeState(new TheaterInfoState()
                        {
                            Id = idInt,
                            Estado = (val.Value) ? 1 : 0,
                        }).Result;

                        if (confirmation.Exists())
                        {
                            e.Handled = false;
                            MessageBox.Show("O estado do teatro foi mudado com sucesso", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            e.Handled = true;
                            MessageBox.Show("Erro ao mudar o estado do teatro", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        channel.ShutdownAsync().Wait();
                    }
                    else
                    {
                        // Erro ao converter o id
                        e.Handled = true;
                        MessageBox.Show("Erro ao mudar o estado do teatro", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    e.Handled = true;
                    MessageBox.Show("Erro ao mudar o estado do teatro", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("Erro ao mudar o estado do teatro", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeEstadoEspetaculo_Click(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = (CheckBox)sender;

            if (checkBox != null)
            {
                string? id = checkBox.Tag.ToString();
                bool? val = checkBox.IsChecked;

                if (id != null && val != null)
                {

                    bool r1 = int.TryParse(id, out int idInt);

                    if (r1)
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new ShowServiceClient(new ShowService.ShowServiceClient(channel));

                        Confirmation confirmation = client.ChangeState(new ShowInfoState()
                        {
                            Id = idInt,
                            Estado = (val.Value) ? 1 : 0,
                        }).Result;

                        if (confirmation.Exists())
                        {
                            e.Handled = false;
                            MessageBox.Show("O estado do espetaculo foi mudado com sucesso", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            e.Handled = true;
                            MessageBox.Show("Erro ao mudar o estado do espetaculo", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        channel.ShutdownAsync().Wait();
                    }
                    else
                    {
                        // Erro ao converter o id
                        e.Handled = true;
                        MessageBox.Show("Erro ao mudar o estado do espetaculo", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    e.Handled = true;
                    MessageBox.Show("Erro ao mudar o estado do espetaculo", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("Erro ao mudar o estado do teatro", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ChangeEstadoSessao_Click(object sender, RoutedEventArgs e)
        {
            CheckBox? checkBox = (CheckBox)sender;

            if (checkBox != null)
            {
                string? id = checkBox.Tag.ToString();
                bool? val = checkBox.IsChecked;

                if (id != null && val != null)
                {

                    bool r1 = int.TryParse(id, out int idInt);

                    if (r1)
                    {
                        var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                        var client = new SessionServiceClient(new SessionService.SessionServiceClient(channel));

                        Confirmation confirmation = client.ChangeState(new SessionInfoState()
                        {
                            Id = idInt,
                            Estado = (val.Value) ? 1 : 0,
                        }).Result;

                        if (confirmation.Exists())
                        {
                            e.Handled = false;
                            MessageBox.Show("O estado da sessao foi mudado com sucesso", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            e.Handled = true;
                            MessageBox.Show("Erro ao mudar o estado da sessao", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                        }

                        channel.ShutdownAsync().Wait();
                    }
                    else
                    {
                        // Erro ao converter o id
                        e.Handled = true;
                        MessageBox.Show("Erro ao mudar o estado da sessao", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    e.Handled = true;
                    MessageBox.Show("Erro ao mudar o estado da sessao", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                e.Handled = true;
                MessageBox.Show("Erro ao mudar o estado da sessao", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTeatros_Click(object sender, RoutedEventArgs e)
        {
            PopUpCompra = Visibility.Visible;

            AddTeatrosPage addTeatrosPage = new AddTeatrosPage(userConnected, ClosePopup);

            PopGrid.Children.Add(new Frame()
            {
                Content = addTeatrosPage,
            });

        }

        private void ModTeatros_Click(object sender, RoutedEventArgs e)
        {
            if (ListTeatro.SelectedItem != null)
            {

                TeatrosForm? teatroForm = ListTeatro.SelectedItem as TeatrosForm;

                if (teatroForm != null)
                {

                    var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                    var clientT = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

                    TheaterInfo theaterInfo = clientT.GetTheater(new TheaterInfo()
                    {
                        Id = teatroForm.Id,
                    }).Result;

                    if (theaterInfo.Exists())
                    {

                        PopUpCompra = Visibility.Visible;

                        ModTeatrosPage modTeatrosPage = new ModTeatrosPage(userConnected, theaterInfo, ClosePopup);

                        PopGrid.Children.Add(new Frame()
                        {
                            Content = modTeatrosPage,
                        });
                    }
                    else
                    {
                        MessageBox.Show("Erro ao mudar o estado do teatro", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    channel.ShutdownAsync().Wait();
                }
            }
            else
            {
                MessageBox.Show("Por favor selecione um teatro.", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddEspetaculos_Click(object sender, RoutedEventArgs e)
        {
            PopUpCompra = Visibility.Visible;

            AddShowsPage addShowsPage = new AddShowsPage(userConnected, ClosePopup);

            PopGrid.Children.Add(new Frame()
            {
                Content = addShowsPage,
            });
        }

        private void ModEspetaculos_Click(object sender, RoutedEventArgs e)
        {
            if (ListEspetaculos.SelectedItem != null)
            {

                EspetaculosForm? showForm = ListEspetaculos.SelectedItem as EspetaculosForm;

                if (showForm != null)
                {

                    var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                    var clientT = new ShowServiceClient(new ShowService.ShowServiceClient(channel));
                    var clientL = new TheaterServiceClient(new TheaterService.TheaterServiceClient(channel));

                    ShowInfo showInfo = clientT.GetShow(new ShowInfo()
                    {
                        Id = showForm.Id,
                    }).Result;

                    IEnumerable<TheaterInfo> theaters = clientL.GetTheaters(userConnected).Result;

                    if (showInfo.Exists())
                    {
                        PopUpCompra = Visibility.Visible;

                        ModShowsPage modShowsPage = new ModShowsPage(userConnected, showInfo, ClosePopup);

                        PopGrid.Children.Add(new Frame()
                        {
                            Content = modShowsPage,
                        });
                    }
                    else
                    {
                        MessageBox.Show("Erro ao mudar o estado do teatro", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    channel.ShutdownAsync().Wait();
                }
            }
            else
            {
                MessageBox.Show("Por favor selecione um teatro.", "TeatrosLand", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddSessoes_Click(object sender, RoutedEventArgs e)
        {
            PopUpCompra = Visibility.Visible;

            AddSessionsPage addSessionsPage = new AddSessionsPage(userConnected, ClosePopup);

            PopGrid.Children.Add(new Frame()
            {
                Content = addSessionsPage,
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public class TeatrosForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Loc { get; set; } = "";
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
        public string nameShow { get; set; } = "";
        public DateTime SessionDate { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public Decimal TotalPlaces { get; set; }
        public bool Estado { get; set; }
    }
}
