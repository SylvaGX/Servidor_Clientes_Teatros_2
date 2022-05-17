// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using GRPCProto;
using Server.Data;

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
                Services = { GRPCProto.Login.BindService(new LoginImpl(context)) },
                Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            };
            server.Start();

            Console.WriteLine("RouteGuide server listening on port " + Port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();

            server.ShutdownAsync().Wait();
        }
    }
}
