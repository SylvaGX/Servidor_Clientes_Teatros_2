// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class UserServiceImpl : UserService.UserServiceBase
    {
        private ServerContext DBcontext;
        
        public UserServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override Task<UserInfo> GetUser(UserConnected request, ServerCallContext context)
        {
            var user = DBcontext.Users.Include(user => user.IdLocalizationNavigation).FirstOrDefault(user => user.Id.Equals(request.Id));

            if (user != null)
            {
                UserInfo userInfo = new UserInfo()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Mail,
                    Fundos = decimal.ToDouble(user.Fundos),
                    Localization = new LocalizationInfo()
                    {
                        Id = user.IdLocalizationNavigation.Id,
                        Name = user.IdLocalizationNavigation.Loc,
                        Lat = user.IdLocalizationNavigation.Lat,
                        Longi = user.IdLocalizationNavigation.Longi,
                    },
                };

                userInfo.Purchases.Clear();
                
                return Task.FromResult(userInfo);
            }

            return Task.FromResult(new UserInfo() { Id = -1 });
        }
    }
}