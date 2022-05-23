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
        readonly CartService.CartServiceClient _client;

        public CarServiceClient(CartService.CartServiceClient client)
        {
            _client = client;
        }

        public async Task<Confirmation> ReservePlaces(SessionInfoReserve sessionInfoReserve)
        {
            try
            {
                Confirmation confirmation = _client.ReservePlaces(sessionInfoReserve);
                
                return await Task.FromResult(confirmation);
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1,
                });
            }
        }
    }

}
