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
