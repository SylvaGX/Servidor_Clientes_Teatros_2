using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for SessoesList.xaml
    /// </summary>
    public partial class UserList : Window
    {
        UserConnected userConnected { get; }
        List<UsersForm> usersForms { get; set; } = new List<UsersForm>();
        IEnumerable<UserInfo> users;

        Timer tickerSessions = new Timer();
        public UserList(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var clientSessions = new UserServiceClient(new UserService.UserServiceClient(channel));

            users = clientSessions.GetUsers(userConnected).Result;

            if (users.Any())
            {
                foreach (UserInfo user in users)
                {
                    usersForms.Add(new UsersForm()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Local = user.Localization.Name
                    });
                }
                UsersList.ItemsSource = usersForms;
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

            users = client.GetUsers(userConnected).Result;

            if (users.Any())
            {

                UsersList.ItemsSource = null;
                usersForms.Clear();
                foreach (UserInfo user in users)
                {
                    usersForms.Add(new UsersForm()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Local = user.Localization.Name
                    });
                }
                UsersList.ItemsSource = usersForms;
            }

            channel.ShutdownAsync().Wait();
        }
        private void Adicionar_Click(object sender, RoutedEventArgs e)
        {
            AddUser addUser = new(userConnected);

            addUser.Show();

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

    public class UsersForm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Local { get; set; } = "";
    }
}
