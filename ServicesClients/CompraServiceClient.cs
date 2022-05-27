using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class CompraServiceClient
    {
        readonly CompraService.CompraServiceClient _client;

        public CompraServiceClient(CompraService.CompraServiceClient client)
        {
            _client = client;
        }

        public async Task<RefCompra> BuySessions(List<SessionInfoCompra> sessionInfoCompra)
        {
            try
            {
                RefCompra refCompra = new RefCompra() { Id = 1 };

                using (var call = _client.BuySessions())
                {
                    var responseReaderTask = Task.Run(() =>
                    {
                        refCompra = call.ResponseAsync.Result;
                    });

                    foreach (SessionInfoCompra session in sessionInfoCompra)
                    {
                        call.RequestStream.WriteAsync(session).Wait();
                    }

                    call.RequestStream.CompleteAsync().Wait();
                    responseReaderTask.Wait();
                }

                return await Task.FromResult(refCompra);
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new RefCompra()
                {
                    Id = -1,
                });
            }
        }

        public async Task<IEnumerable<PurchaseInfo>> GetPurchases(UserConnected userConnected)
        {
            try
            {
                List<PurchaseInfo> purchases = new();

                using (var call = _client.GetPurchases(userConnected))
                {
                    var responseStream = call.ResponseStream;

                    while (responseStream.MoveNext().Result)
                    {
                        purchases.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(purchases);
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new List<PurchaseInfo>().AsEnumerable());
            }
        }

        public async Task<IEnumerable<PurchaseInfo>> HistoryUser(UserConnected userConnected)
        {
            try
            {
                List<PurchaseInfo> purchases = new();

                using (var call = _client.HistoryUser(userConnected))
                {
                    var responseStream = call.ResponseStream;

                    while (responseStream.MoveNext().Result)
                    {
                        purchases.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(purchases);
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new List<PurchaseInfo>().AsEnumerable());
            }
        }
    }
}

