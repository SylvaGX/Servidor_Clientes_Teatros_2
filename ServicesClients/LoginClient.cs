using Grpc.Core;
using gRPCProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_User.Models
{
    public class LoginClient
    {
        readonly Channel _channel;
        readonly Login.LoginClient _client;

        public LoginClient(Channel channel, Login.LoginClient client)
        {
            _client = client;
            _channel = channel;
        }

        public async Task<UserConnected> CheckLogin(UserLogin userLogin)
        {
            try
            {
                //log inicio funcao 
                UserConnected user = _client.CheckLogin(userLogin);
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
                    Msg = $"'RpcException': [{DateTime.Now}] - Error - Erro ao fazer login. RpcException.\nCode Msg: {ex.Message}",
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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao fazer login.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                });
                return await Task.FromResult(new UserConnected() { Id = -1 });
            }
        }
    }

}
