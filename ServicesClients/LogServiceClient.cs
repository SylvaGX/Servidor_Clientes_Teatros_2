using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class LogServiceClient
    {
        readonly Channel _channel;
        readonly LogService.LogServiceClient _client;

        public LogServiceClient(Channel channel, LogService.LogServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<Confirmation> LogInformation(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogInformation(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao enviar o Information Log. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao enviar o Information Log.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }
        public async Task<Confirmation> LogWarning(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogWarning(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao enviar o Warning Log. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao enviar o Warning Log.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<Confirmation> LogError(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogError(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao enviar o Error Log. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao enviar o Error Log.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<Confirmation> LogCritical(LogInfo logInfo)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.LogCritical(logInfo);
                if (confirmation.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(confirmation);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao enviar o Critical Log. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao enviar o Critical Log.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation()
                {
                    Id = -1
                });
            }
        }

        public async Task<IEnumerable<LogInfo>> GetAllLogs(UserConnected userConnected)
        {
            try
            {
                List<LogInfo> logs = new();
                using (var call = _client.getAllLogs(userConnected))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        logs.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(logs.AsEnumerable());
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber os logs. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<LogInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber os logs.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<LogInfo>().AsEnumerable());
            }
        }
    }
}
