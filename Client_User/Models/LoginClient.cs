﻿using Grpc.Core;
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
        readonly gRPCProto.Login.LoginClient _client;

        public LoginClient(gRPCProto.Login.LoginClient login)
        {
            _client = login;
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
            catch (RpcException e)
            {
                //logs error
                Console.Error.WriteLine(e.Message);
                throw;
            }
        }
    }

}
