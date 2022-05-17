// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;

namespace Server
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
                UserConnected user1 = new UserConnected { Id = user.Id, Key = "1", Type = user.Type};
                return Task.FromResult(user1);
            }

            return Task.FromResult(new UserConnected { Id = -1, Key = "", Type = ""});
        }
    }
}