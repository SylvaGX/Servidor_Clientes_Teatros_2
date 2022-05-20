// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;

namespace Server.Models
{
    internal class RegisterImpl : Register.RegisterBase
    {
        private ServerContext DBcontext;
        
        public RegisterImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override Task<UserConnected> RegisterUser(UserRegister request, ServerCallContext context)
        {

            if(DBcontext.Users.FirstOrDefault(user => user.Mail.Equals(request.Email)) == null)
            {

                var user = new User()
                {
                    Name = request.Name,
                    Mail = request.Email,
                    Pass = request.Password,
                    Type = "1",
                    IdLocalization = request.IdLocalization,
                };

                DBcontext.Users.Add(user);

                DBcontext.SaveChanges();

                Console.WriteLine($"Hello: {user.Id}");

                user = DBcontext.Users.FirstOrDefault(user => user.Mail.Equals(request.Email));

                if(user != null)
                {
                    return Task.FromResult(new UserConnected()
                    {
                        Id = user.Id,
                        Type = user.Type,
                    });
                }

                return Task.FromResult(new UserConnected()
                {
                    Id = -1,
                    Type = "",
                });
            }

            return Task.FromResult(new UserConnected()
            {
                Id = -1,
                Type = "",
            });
        }
    }
}