using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class SessionServiceClient
    {
        readonly SessionService.SessionServiceClient _client;

        public SessionServiceClient(SessionService.SessionServiceClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<SessionInfo>> GetAllSessions(UserConnected userConnected)
        {
            try
            {
                List<SessionInfo> shows = new();
                using (var call = _client.GetAllSessions(userConnected))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        shows.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(shows.AsEnumerable());
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new List<SessionInfo>().AsEnumerable());
            }
        }

        public async Task<IEnumerable<SessionInfo>> GetSessions(ShowInfo show)
        {
            try
            {
                List<SessionInfo> shows = new();
                using (var call = _client.GetSessions(show))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        shows.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(shows.AsEnumerable());
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new List<SessionInfo>().AsEnumerable());
            }
        }
    }

}
