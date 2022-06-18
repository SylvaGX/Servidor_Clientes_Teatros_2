using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
    /// Interaction logic for Carrinho.xaml
    /// </summary>
    public partial class Carrinho : Window, INotifyPropertyChanged
    {
        public UserConnected userConnected;

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
        public Carrinho(UserConnected userConnected)
        {
            InitializeComponent();

            this.userConnected = userConnected;

            Init();
        }

        private void Init()
        {
            decimal TotalPrice = 0;

            foreach (var p in App.carrinho)
            {
                TotalPrice += p.Price * p.NumberPlaces;
            }

            Total.Text = "Total: " + TotalPrice.ToString("0.00") + " €";

            ShoppingCarList.ItemsSource = App.carrinho;

        }

        private void Voltar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(userConnected);

            mainWindow.Show();

            Close();
        }

        private void Limpar_Click(object sender, RoutedEventArgs e)
        {

            var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
            var client = new CarServiceClient(channel, new CartService.CartServiceClient(channel));

            Confirmation confirmation;

            foreach (SessionInfoForm sessionForm in App.carrinho)
            {
                if (sessionForm.NumberPlaces > 0)
                {
                    confirmation = client.CancelReservationPlaces(new SessionInfoReserve()
                    {
                        Id = sessionForm.Id,
                        NumberPlaces = sessionForm.NumberPlaces,
                    }).Result;

                    if (!confirmation.Exists())
                    {
                        // Erro ao encontrar a sessao na BD
                    }
                }
            }

            channel.ShutdownAsync().Wait();

            App.carrinho.Clear();

            ShoppingCarList.ItemsSource = null;

            Init();
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string? s = button.Tag.ToString();
                if (s != null)
                {
                    string[] vals = s.Split(',');
                    if(vals.Length == 2)
                    {
                        var r1 = int.TryParse(vals[0].ToString(), out int n);
                        var r2 = int.TryParse(vals[1].ToString(), out int p);
                        if (r1 && r2)
                        {
                            SessionInfoForm? sessionInfoForm = App.carrinho.FirstOrDefault(c => c.Id == n && c.NumberPlaces == p);

                            if (sessionInfoForm != null)
                            {
                                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                                var client = new CarServiceClient(channel, new CartService.CartServiceClient(channel));

                                Confirmation confirmation = client.CancelReservationPlaces(new SessionInfoReserve()
                                {
                                    Id = sessionInfoForm.Id,
                                    NumberPlaces = sessionInfoForm.NumberPlaces,
                                }).Result;

                                if (confirmation.Exists())
                                {
                                    if (confirmation.Id == 1)
                                    {
                                        App.carrinho.Remove(sessionInfoForm);

                                        ShoppingCarList.ItemsSource = null;

                                        Init();
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

                                channel.ShutdownAsync().Wait();
                            }
                        }
                    }
                }
            }
        }

        public void ClosePopup()
        {
            PopUpCompra = Visibility.Hidden;
            PopGrid.Children.Clear();

            MainWindow mainWindow = new(userConnected);

            mainWindow.Show();

            Close();
        }

        private void Comprar_Click(object sender, RoutedEventArgs e)
        {
            if(App.carrinho.Count > 0)
            {
                var channel = new Channel(App.IPAdd, ChannelCredentials.Insecure);
                var client = new CompraServiceClient(channel, new CompraService.CompraServiceClient(channel));

                RefCompra refCompra;

                List<SessionInfoCompra> sessions = new List<SessionInfoCompra>();

                foreach(var item in App.carrinho)
                {
                    sessions.Add(new SessionInfoCompra()
                    {
                        Id = item.Id,
                        UserId = userConnected.Id,
                        NumberPlaces = item.NumberPlaces,
                        Price = decimal.ToDouble(item.Price),
                    });
                }

                refCompra = client.BuySessions(sessions).Result;

                if (refCompra.Exists())
                {
                    if(refCompra.Id > 0)
                    {
                        App.carrinho.Clear();

                        ShoppingCarList.ItemsSource = null;

                        PopUpCompra = Visibility.Visible;

                        ReferenciasPage referenciasPage = new ReferenciasPage(refCompra, userConnected, ClosePopup);

                        PopGrid.Children.Add(new Frame()
                        {
                            Content = referenciasPage,
                        });

                    }
                    else
                    {
                        // Error don't have money
                        MessageBox.Show("Você não tem dinheiro para a compra que está a efetuar!", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    // Erro ao encontrar a sessao na BD
                    MessageBox.Show("Ocorreu um erro. Iremos resolver o problema.", "TeatroLand", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                channel.ShutdownAsync().Wait();

            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
