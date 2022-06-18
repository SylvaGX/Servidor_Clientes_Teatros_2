using Grpc.Core;
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
        readonly Channel _channel;
        readonly ShowService.ShowServiceClient _client;

        public ShowServiceClient(Channel channel, ShowService.ShowServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<Confirmation> AddShow(ShowInfo showInfo)
        {
            try
            {
                Confirmation confirmation = _client.AddShow(showInfo);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao adicionar o show. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar o show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<Confirmation> UpdateShow(ShowInfo showInfo)
        {
            try
            {
                Confirmation confirmation = _client.UpdateShow(showInfo);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao atualizar o show. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao atualizar o show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<ShowInfo> GetShow(ShowInfo showInfo)
        {
            try
            {
                ShowInfo show = _client.GetShow(showInfo);

                return await Task.FromResult(show);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber o show. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new ShowInfo() { Id = -1 });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber o show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new ShowInfo() { Id = -1 });
            }
        }

        public async Task<Confirmation> ChangeState(ShowInfoState showInfoState)
        {
            try
            {
                Confirmation confirmation = _client.ChangeState(showInfoState);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao mudar o estado do show. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao mudar o estado do show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation() { Id = -1 });
            }
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
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber os shows. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<ShowInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber os shows.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<ShowInfo>().AsEnumerable());
            }
        }
    }
}
