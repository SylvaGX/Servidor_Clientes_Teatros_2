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
    public partial class ManagerList : Window
    {
        UserConnected userConnected { get; }
        List<ManagersForm> usersForms { get; set; } = new List<ManagersForm>();
        IEnumerable<UserInfo> users;

        Timer tickerSessions = new Timer();
        public ManagerList(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientSessions = new UserServiceClient(new UserService.UserServiceClient(channel));

            users = clientSessions.GetManagers(userConnected).Result;

            if (users.Any())
            {
                foreach (UserInfo user in users)
                {
                    usersForms.Add(new ManagersForm()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Local = user.Localization.Name
                    });
                }
                ManagersList.ItemsSource = usersForms;
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
                UpdateManagerList();
            }), null);
        }

        public void UpdateManagerList()
        {
            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new UserServiceClient(new UserService.UserServiceClient(channel));

            users = client.GetManagers(userConnected).Result;

            if (users.Any())
            {

                ManagersList.ItemsSource = null;
                usersForms.Clear();
                foreach (UserInfo user in users)
                {
                    usersForms.Add(new ManagersForm()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Local = user.Localization.Name
                    });
                }
                ManagersList.ItemsSource = usersForms;
            }

            channel.ShutdownAsync().Wait();
        }

        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            AddManager addManager = new(userConnected);

            addManager.Show();

            Close();
        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            tickerSessions.Stop();
            tickerSessions.Close();

            MainWindow mainWindow = new(userConnected);

            mainWindow.Show();

            Close();
        }
    }

    public class ManagersForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Local { get; set; } = "";
    }
}
