using Client_User.Models;
using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client_User
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string IPAdd { get; set; } = "10.144.10.2:45300";
        public static List<SessionInfoForm> carrinho { get; set; } = new List<SessionInfoForm>();
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (carrinho.Count > 0)
            {
                var channel = new Channel(IPAdd, ChannelCredentials.Insecure);
                var client = new CarServiceClient(channel, new CartService.CartServiceClient(channel));

                Confirmation confirmation;

                foreach (SessionInfoForm session in carrinho)
                {
                    confirmation = client.CancelReservationPlaces(new SessionInfoReserve()
                    {
                        Id = session.Id,
                        NumberPlaces = session.NumberPlaces,
                    }).Result;

                    if (!confirmation.Exists()) { 
                    
                        // Erro ao encontrar a sessao na BD
                    }
                }

                carrinho.Clear();

                channel.ShutdownAsync().Wait();
            }
        }
    }
}
