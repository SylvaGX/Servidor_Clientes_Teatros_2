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
        readonly Channel _channel;
        readonly CompraService.CompraServiceClient _client;

        public CompraServiceClient(Channel channel, CompraService.CompraServiceClient client)
        {
            _client = client;
            _channel = channel;
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
            catch (ArgumentNullException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao comprar sessões. ArgumentNullException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new RefCompra()
                {
                    Id = -1,
                });
            }
            catch (AggregateException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'AggregateException': [{DateTime.Now}] - Error - Erro ao comprar sessões. AggregateException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new RefCompra()
                {
                    Id = -1,
                });
            }
            catch (ObjectDisposedException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'ObjectDisposedException': [{DateTime.Now}] - Error - Erro ao comprar sessões. ObjectDisposedException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new RefCompra()
                {
                    Id = -1,
                });
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao comprar sessões. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new RefCompra()
                {
                    Id = -1,
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao comprar sessões.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new RefCompra()
                {
                    Id = -1,
                });
            }
        }

        public async Task<Confirmation> PayPurchases(RefCompra refe)
        {
            try
            {
                Confirmation confirmation = _client.PayPurchases(refe);

                return confirmation;
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber as compras do utilizador. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation() { Id = -1 });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as compras do utilizador.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation() { Id = -1 });
            }
        }

        public async Task<Confirmation> CancelPurchase(PurchaseInfo purchaseInfo)
        {
            try
            {
                Confirmation confirmation = _client.CancelPurchase(purchaseInfo);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao cancelar as compras.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation() { Id = -1 });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao cancelar as compras.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation() { Id = -1 });
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
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber as compras do utilizador. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<PurchaseInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as compras do utilizador.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<PurchaseInfo>().AsEnumerable());
            }
        }

        public async Task<PurchaseInfo> GetPurchase(PurchaseInfo request)
        {
            try
            {
                PurchaseInfo purchaseInfo = _client.GetPurchase(request);

                return await Task.FromResult(purchaseInfo);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber o histórico de compras. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new PurchaseInfo() { Id = -1 });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber o histórico de compras.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new PurchaseInfo() { Id = -1 });
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
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber o histórico de compras. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<PurchaseInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber o histórico de compras.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<PurchaseInfo>().AsEnumerable());
            }
        }
    }
}

