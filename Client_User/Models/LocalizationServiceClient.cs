using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class LocalizationServiceClient
    {
        readonly LocalizationService.LocalizationServiceClient _client;

        public LocalizationServiceClient(LocalizationService.LocalizationServiceClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<LocalizationInfo>> GetLocalizations(UserConnected user)
        {
            try
            {
                List<LocalizationInfo> localizations = new();
                using (var call = _client.GetLocalizations(user))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        localizations.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(localizations.AsEnumerable());
            }
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new List<LocalizationInfo>().AsEnumerable());
            }
        }
    }

}
