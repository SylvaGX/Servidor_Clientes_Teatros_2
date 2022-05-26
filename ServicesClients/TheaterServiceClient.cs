using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class TheaterServiceClient
    {
        readonly TheaterService.TheaterServiceClient _client;

        public TheaterServiceClient(TheaterService.TheaterServiceClient client)
        {
            _client = client;
        }

        public async Task<Confirmation> AddTheater(TheaterInfo theaterInfo)
        {
            try
            {
                Confirmation confirmation = _client.AddTheater(theaterInfo);

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

        public async Task<Confirmation> UpdateTheater(TheaterInfo theaterInfo)
        {
            try
            {
                Confirmation confirmation = _client.UpdateTheater(theaterInfo);

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

        public async Task<TheaterInfo> GetTheater(TheaterInfo theaterInfo)
        {
            try
            {
                TheaterInfo theater = _client.GetTheater(theaterInfo);

                return await Task.FromResult(theater);
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new TheaterInfo()
                {
                    Id = -1,
                });
            }

        }

        public async Task<Confirmation> ChangeState(TheaterInfoState theaterInfoState)
        {
            try
            {
                Confirmation confirmation = _client.ChangeState(theaterInfoState);

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

        public async Task<IEnumerable<TheaterInfo>> GetTheaters(UserConnected userConnected)
        {
            try
            {
                List<TheaterInfo> theaters = new();
                using (var call = _client.GetTheaters(userConnected))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        theaters.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(theaters.AsEnumerable());
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new List<TheaterInfo>().AsEnumerable());
            }
        }
    }
}
