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

        private void ListTheaters_Click(object sender, RoutedEventArgs e)
        {
            TeatrosList teatrosList = new TeatrosList(userConnected);

            teatrosList.Show();

            Close();
        }

        private void ListShows_Click(object sender, RoutedEventArgs e)
        {
            EspetaculoList espetaculoList = new EspetaculoList(userConnected);

            espetaculoList.Show();

            Close();
        }

        private void ListSessions_Click(object sender, RoutedEventArgs e)
        {
            SessoesList sessoesList = new SessoesList(userConnected);

            sessoesList.Show();

            Close();
        }

        private void ListPurchases_Click(object sender, RoutedEventArgs e)
        {
            ComprasList comprasList = new ComprasList(userConnected);

            comprasList.Show();

            Close();
        }
    }
}
