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

namespace Client_Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserConnected userConnected { get; }
        public MainWindow(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void ListTheaters_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TeatrosList teatrosList = new(userConnected);

                teatrosList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de teatros.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListShows_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EspetaculoList espetaculoList = new(userConnected);

                espetaculoList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de espetáculos.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListSessions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SessoesList sessoesList = new(userConnected);

                sessoesList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de sessões.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListPurchases_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComprasList comprasList = new(userConnected);

                comprasList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de compras.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListManager_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ManagerList managerList = new(userConnected);

                managerList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de managers.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ListUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserList userList = new(userConnected);

                userList.Show();

                Close();
            }
            catch (Exception ex)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });

                MessageBox.Show($"Ocorreu um erro ao redirecionar para a lista de users.", "MainWindow", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
