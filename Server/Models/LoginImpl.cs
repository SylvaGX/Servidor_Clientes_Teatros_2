// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;

namespace Server.Models
{
    internal class LoginImpl : Login.LoginBase
    {
        private ServerContext DBcontext;
        
        public LoginImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override Task<UserConnected> CheckLogin(UserLogin request, ServerCallContext context)
        {

            var user = DBcontext.Users.FirstOrDefault(user => user.Mail.Equals(request.Email) && user.Pass.Equals(request.Password));

            if (user != null)
            {
                if (user.Type.Equals(request.Type))
                {
                    UserConnected userConnected = new UserConnected { Id = user.Id, Type = user.Type };
                    return Task.FromResult(userConnected);
                }
                else
                {
                    return Task.FromResult(new UserConnected { Id = -2, Type = "" });
                }
            }

            return Task.FromResult(new UserConnected { Id = -1, Type = ""});
        }
    }
}