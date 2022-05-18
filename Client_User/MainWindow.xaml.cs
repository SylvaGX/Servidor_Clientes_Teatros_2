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
        public static string IPAdd { get; set; }
        public UserConnected userConnected { get; }
    
        public MainWindow(UserConnected userConnected)
        {
            InitializeComponent();
            this.userConnected = userConnected;

            if(IPAdd != null) { 
                var channel = new Channel(IPAdd + ":45300", ChannelCredentials.Insecure);
                
                var client = new UserServiceClient(new UserService.UserServiceClient(channel));
                UserInfo userInfo = client.GetUser(userConnected).Result;
                Email.Text = userInfo.Email.ToString();
                Fundos.Text = userInfo.Fundos.ToString();

                channel.ShutdownAsync().Wait();
            }
            else
            {
                Login login = new Login();
                login.Show();
                this.Close();
            }

        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
