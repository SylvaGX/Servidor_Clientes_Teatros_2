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

namespace Client_User
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UserConnected userConnected { get; }
    
        public MainWindow(UserConnected userConnected)
        {
            InitializeComponent();
            this.userConnected = userConnected;

            if(App.IPAdd != null) { 
                var channel = new Channel(App.IPAdd + ":45300", ChannelCredentials.Insecure);
                var client = new UserServiceClient(new UserService.UserServiceClient(channel));

                UserInfo userInfo = client.GetUser(userConnected).Result;
                Email.Text = userInfo.Email.ToString();
                Fundos.Text = Convert.ToDecimal(userInfo.Fundos).ToString("0.00") + " €";

                channel.ShutdownAsync().Wait();
            }
            else
            {
                Login login = new();
                login.Show();
                Close();
            }

        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ListTeatros_Click(object sender, RoutedEventArgs e)
        {
            TeatrosLista teatrosLista = new(userConnected);
            teatrosLista.Show();
            Close();
        }

        private void Carrinho_Click(object sender, RoutedEventArgs e)
        {
            Carrinho car = new Carrinho(userConnected);

            car.Show();

            Close();
        }

        private void HistoricoCompras_Click(object sender, RoutedEventArgs e)
        {
            TeatrosHist teatrosHist = new TeatrosHist(userConnected);

            teatrosHist.Show();

            Close();
        }
    }
}
