using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class UserServiceClient
    {
        readonly Channel _channel;
        readonly UserService.UserServiceClient _client;

        public UserServiceClient(Channel channel, UserService.UserServiceClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<IEnumerable<UserInfo>> GetUsers(UserConnected userConnected)
        {
            try
            {
                List<UserInfo> users = new();
                using (var call = _client.GetUsers(userConnected))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        users.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(users.AsEnumerable());
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber os users. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<UserInfo>().AsEnumerable());
            }
            catch(Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber os users.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<UserInfo>().AsEnumerable());
            }
        }

        public async Task<IEnumerable<UserInfo>> GetManagers(UserConnected userConnected)
        {
            try
            {
                List<UserInfo> users = new();
                using (var call = _client.GetManagers(userConnected))
                {
                    var responseStream = call.ResponseStream;
                    while (responseStream.MoveNext().Result)
                    {
                        users.Add(call.ResponseStream.Current);
                    }
                }

                return await Task.FromResult(users.AsEnumerable());
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber os managers. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<UserInfo>().AsEnumerable());
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber os managers.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new List<UserInfo>().AsEnumerable());
            }
        }

        public async Task<UserInfo> GetUser(UserConnected userConnected)
        {
            try
            {
                //log inicio funcao 
                UserInfo user = _client.GetUser(userConnected);
                if (user.Exists())
                {
                    //log a dizer que funcionou
                }
                else
                {
                    //log erro
                }

                return await Task.FromResult(user);
            }
            catch (RpcException ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao receber o user. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new UserInfo() { Id = -1 });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber o user.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new UserInfo() { Id = -1 });
            }
        }

        public async Task<Confirmation> AddMoney(UserAddMoney userAddMoney)
        {
            try
            {
                //log inicio funcao 
                Confirmation confirmation = _client.AddMoney(userAddMoney);

                if (confirmation.Exists())
                {
                    if (confirmation.Id == 1)
                    {
                        //log a dizer que funcionou
                    }
                    else
                    {
                        // Erro ao tranferir o dinheiro
                    }
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
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao adicionar o dinheiro. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar o dinheiro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new Confirmation() { Id = -1 });
            }
        }
    }

}
