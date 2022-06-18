using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class RegisterClient
    {
        readonly Channel _channel;
        readonly Register.RegisterClient _client;

        public RegisterClient(Channel channel, Register.RegisterClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<UserConnected> RegisterManager(ManagerRegister userRegister)
        {
            try
            {
                //log inicio funcao 
                UserConnected user = _client.RegisterManager(userRegister);
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
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao registar o manager. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new UserConnected() { Id = -1 });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao registar o manager.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new UserConnected() { Id = -1 });
            }
        }

        public async Task<UserConnected> RegisterUser(UserRegister userRegister)
        {
            try
            {
                //log inicio funcao 
                UserConnected user = _client.RegisterUser(userRegister);
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
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao registar o user. RpcException.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new UserConnected() { Id = -1 });
            }
            catch (Exception ex)
            {
                //logs error
                var logClient = new LogServiceClient(_channel, new LogService.LogServiceClient(_channel));
                await logClient.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao registar o user.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new UserConnected() { Id = -1 });
            }
        }
    }
}
