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
        readonly gRPCProto.Register.RegisterClient _client;

        public RegisterClient(gRPCProto.Register.RegisterClient client)
        {
            _client = client;
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e.Message);
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e.Message);
                return await Task.FromResult(new UserConnected() { Id = -1 });
            }
        }
    }

}
