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

        public override async Task GetUsers(UserConnected request, IServerStreamWriter<UserInfo> responseStream, ServerCallContext context)
        {
            var users = DBcontext.Users.Include(u => u.IdLocalizationNavigation).Where(u => u.Type.Equals("1"));
            UserInfo u;

            foreach (var user in users)
            {
                u = new UserInfo()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Mail,
                    Localization = new LocalizationInfo()
                    {
                        Id = user.IdLocalizationNavigation.Id,
                        Name = user.IdLocalizationNavigation.Loc,
                        Lat = user.IdLocalizationNavigation.Lat,
                        Longi = user.IdLocalizationNavigation.Longi,
                    },
                };

                await responseStream.WriteAsync(u);
            }
        }
        public override async Task GetManagers(UserConnected request, IServerStreamWriter<UserInfo> responseStream, ServerCallContext context)
        {
            var users = DBcontext.Users.Include(u => u.IdLocalizationNavigation).Where(u => u.Type.Equals("2"));
            UserInfo u;

            foreach (var user in users)
            {
                u = new UserInfo()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Mail,
                    Localization = new LocalizationInfo()
                    {
                        Id = user.IdLocalizationNavigation.Id,
                        Name = user.IdLocalizationNavigation.Loc,
                        Lat = user.IdLocalizationNavigation.Lat,
                        Longi = user.IdLocalizationNavigation.Longi,
                    },
                };

                await responseStream.WriteAsync(u);
            }
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

        public override async Task<Confirmation> AddMoney(UserAddMoney request, ServerCallContext context)
        {

            var user = DBcontext.Users.FirstOrDefault(u => u.Id.Equals(request.User.Id));

            Confirmation confirmation = new Confirmation() { Id = 0 };

            if(user != null)
            {
                user.Fundos += Convert.ToDecimal(request.MoneyToAdd);

                DBcontext.SaveChanges();

                confirmation.Id = 1;
            }


            return await Task.FromResult(confirmation);
        }
    }
}