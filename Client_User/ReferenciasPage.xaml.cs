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
    /// Interaction logic for ReferenciasPage.xaml
    /// </summary>
    public partial class ReferenciasPage : Page
    {
        UserConnected UserConnected { get; }
        RefCompra RefCompra { get; }
        Action Close { get; }


        public ReferenciasPage(RefCompra refe, UserConnected userConnected, Action close)
        {
            InitializeComponent();

            RefCompra = refe;
            UserConnected = userConnected;
            Close = close;

            RefText.Text = RefCompra.Reference;
            TotalPreco.Text = RefCompra.PrecoTotal.ToString() + " €";
        }

        private async void Pagar_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new CompraServiceClient(channel, new CompraService.CompraServiceClient(channel));

                Confirmation confirmation = client.PayPurchases(RefCompra).Result;

                if (confirmation.Exists())
                {

                    MessageBox.Show("Pagamento efetuado com sucesso.", "Carrinho", MessageBoxButton.OK, MessageBoxImage.Information);

                    Close.Invoke();

                }
                else
                {

                    MessageBox.Show("Ocorreu um erro ao tentar pagar as sessões. tente novamente na aba \"Histórico de Compras\".", "Carrinho", MessageBoxButton.OK, MessageBoxImage.Warning);

                    Close.Invoke();

                }
            }
            catch (Exception ex)
            {
                //logs error
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var logClient = new LogServiceClient(channel, new LogService.LogServiceClient(channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as compras do utilizador.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {

            Close.Invoke();

        }
    }
}
