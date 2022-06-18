using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class CarServiceClient
    {
        readonly Channel _channel;
        readonly CartService.CartServiceClient _client;

        public CarServiceClient(Channel channel, CartService.CartServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<Confirmation> ReservePlaces(SessionInfoReserve sessionInfoReserve)
        {
            try
            {
                Confirmation confirmation = _client.ReservePlaces(sessionInfoReserve);
                
                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao reservar os lugares. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao reservar os lugares.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<Confirmation> CancelReservationPlaces(SessionInfoReserve sessionInfoReserve)
        {
            try
            {
                Confirmation confirmation = _client.CancelReservationPlaces(sessionInfoReserve);
                
                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao cancelar a reserva dos lugares. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao cancelar a reserva dos lugares.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }
    }

}
