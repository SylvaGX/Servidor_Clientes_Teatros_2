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
        readonly UserService.UserServiceClient _client;

        public UserServiceClient(UserService.UserServiceClient client)
        {
            _client = client;
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e);
                return await Task.FromResult(new Confirmation() { Id = -1 });
            }
        }
    }

}
