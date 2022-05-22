// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class TheaterServiceImpl : TheaterService.TheaterServiceBase
    {
        private ServerContext DBcontext;
        
        public TheaterServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override async Task GetTheaters(UserConnected request, IServerStreamWriter<TheaterInfo> responseStream, ServerCallContext context)
        {
            var theaters = DBcontext.Theaters.Include(t => t.IdLocalizationNavigation);
            TheaterInfo t;

            foreach (var theater in theaters)
            {
                t = new TheaterInfo()
                {
                    Id = theater.Id,
                    Name = theater.Name,
                    Address = theater.Address,
                    Contact = theater.Contact,
                    Localization = new LocalizationInfo()
                    {
                        Id = theater.IdLocalizationNavigation.Id,
                        Name = theater.IdLocalizationNavigation.Loc,
                        Lat = theater.IdLocalizationNavigation.Lat,
                        Longi = theater.IdLocalizationNavigation.Longi,
                    },
                };

                await responseStream.WriteAsync(t);
            }
        }
    }
}