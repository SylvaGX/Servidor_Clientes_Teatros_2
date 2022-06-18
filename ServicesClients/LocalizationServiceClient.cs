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
        readonly Channel _channel;
        readonly LocalizationService.LocalizationServiceClient _client;

        public LocalizationServiceClient(Channel channel, LocalizationService.LocalizationServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<Confirmation> AddLocalization(LocalizationInfo localizationInfo)
        {
            try
            {
                Confirmation confirmation = _client.AddLocalization(localizationInfo);

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
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber as localizações. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<LocalizationInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as localizações.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<LocalizationInfo>().AsEnumerable());
            }
        }
    }

}
