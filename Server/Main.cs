﻿// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using gRPCProto;
using Server.Data;
using Server.Models;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            const int Port = 45300;
            
            var context = new ServerContext();
            
            Grpc.Core.Server server = new Grpc.Core.Server
            {
                Services = { 
                    Login.BindService(new LoginImpl(context)),
                    UserService.BindService(new UserServiceImpl(context)),
                    Register.BindService(new RegisterImpl(context)),
                    LocalizationService.BindService(new LocalizationServiceImpl(context)),
                    TheaterService.BindService(new TheaterServiceImpl(context)),
                    ShowService.BindService(new ShowServiceImpl(context)),
                    SessionService.BindService(new SessionServiceImpl(context)),
                    CartService.BindService(new CartServiceImpl(context)),
                    CompraService.BindService(new CompraServiceImpl(context)),
                },
                Ports = { new ServerPort("10.144.10.2", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RouteGuide server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
