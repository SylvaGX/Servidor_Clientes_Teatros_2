﻿using Grpc.Core;
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

        public UserServiceClient(UserService.UserServiceClient userServiceClient)
        {
            _client = userServiceClient;
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
    }

}
