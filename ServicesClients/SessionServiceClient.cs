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
        readonly Channel _channel;
        readonly SessionService.SessionServiceClient _client;

        public SessionServiceClient(Channel channel, SessionService.SessionServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<Confirmation> AddSession(SessionInfo sessionInfo)
        {
            try
            {
                Confirmation confirmation = _client.AddSession(sessionInfo);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao adicionar a sessão. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar a sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<Confirmation> UpdateSession(SessionInfo sessionInfo)
        {
            try
            {
                Confirmation confirmation = _client.UpdateSession(sessionInfo);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao atualizar a sessão. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao atualizar a sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }
        public async Task<Confirmation> ChangeState(SessionInfoState sessionInfoState)
        {
            try
            {
                Confirmation confirmation = _client.ChangeState(sessionInfoState);

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao mudar o estado da sessão. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao mudar o estado da sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
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
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber todas as sessões. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<SessionInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber todas as sessões.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
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
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber as sessões do show. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<SessionInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as sessões do show..\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<SessionInfo>().AsEnumerable());
            }
        }
    }

}
