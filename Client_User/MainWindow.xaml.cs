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
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new UserServiceClient(new UserService.UserServiceClient(channel));

                UserInfo userInfo = client.GetUser(userConnected).Result;
                if (userInfo.Exists())
                {
                    Email.Text = userInfo.Email.ToString();
                    Fundos.Text = Convert.ToDecimal(userInfo.Fundos).ToString("0.00") + " €";
                }
                else
                {
                    // Erro ao obter a informação do utilizador
                }

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

        private void AddMoney_Click(object sender, RoutedEventArgs e)
        {
            string m = AmountMoney.Text;

            var r = decimal.TryParse(m, out decimal md);

            if (r)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new UserServiceClient(new UserService.UserServiceClient(channel));

                Confirmation confirmation = client.AddMoney(new UserAddMoney()
                {
                    User = userConnected,
                    MoneyToAdd = decimal.ToDouble(md),
                }).Result;

                if(confirmation.Exists())
                {
                    if(confirmation.Id == 1)
                    {
                        // Success
                        MessageBox.Show("Transação realizada com sucesso. A quantia desejada foi depositada com sucesso.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        UserInfo userInfo = client.GetUser(userConnected).Result;
                        if (userInfo.Exists())
                        {
                            Fundos.Text = Convert.ToDecimal(userInfo.Fundos).ToString("0.00") + " €";
                        }
                        else
                        {
                            // Erro ao obter a informação do utilizador
                        }
                    }
                    else
                    {
                        // Erro na transação
                        MessageBox.Show("Erro na transação do dinheiro. Não foi possível adiconar a quantia desejável. Tente Novamente.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {

                }

                channel.ShutdownAsync().Wait();
            }
            else
            {
                // Erro ao converter a string do dinheiro em decimal
                MessageBox.Show("Quantia de dinheiro no formato incorreto.\nSe digitou um número decimal com '.'(Ponto) troque por uma ','(Vírgula)\nExemplo: \nErrado: 2.5\nCorreto: 2,5", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
