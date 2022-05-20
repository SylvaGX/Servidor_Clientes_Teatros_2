﻿using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class ShowServiceClient
    {
        readonly ShowService.ShowServiceClient _client;

        public ShowServiceClient(ShowService.ShowServiceClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<ShowInfo>> GetShows(UserConnected user)
        {
            try
            {
                List<ShowInfo> shows = new();
                using (var call = _client.GetShows(user))
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
                return await Task.FromResult(new List<ShowInfo>().AsEnumerable());
            }
        }
    }

}
