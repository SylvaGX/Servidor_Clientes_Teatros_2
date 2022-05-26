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

        public override async Task<Confirmation> AddTheater(TheaterInfo request, ServerCallContext context)
        {
            Theater theater = new Theater()
            {
                Name = request.Name,
                Address = request.Address,
                Contact = request.Contact,
                IdLocalization = request.Localization.Id,
                Estado = 1,
            };

            DBcontext.Add(theater);

            DBcontext.SaveChanges();

            return await Task.FromResult(new Confirmation() { Id = 1 });
        }

        public override async Task<Confirmation> UpdateTheater(TheaterInfo request, ServerCallContext context)
        {
            var theater = DBcontext.Theaters.FirstOrDefault(x => x.Id == request.Id);

            if (theater != null)
            {
                theater.Name = request.Name;
                theater.Address = request.Address;
                theater.Contact = request.Contact;
                theater.IdLocalization = request.Localization.Id;
                theater.Estado = request.Estado;

                DBcontext.Update(theater);

                DBcontext.SaveChanges();

                return await Task.FromResult(new Confirmation() { Id = 1 });
            }
            
            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<TheaterInfo> GetTheater(TheaterInfo request, ServerCallContext context)
        {
            var theater = DBcontext.Theaters.Include(t => t.IdLocalizationNavigation).FirstOrDefault(t => t.Id.Equals(request.Id));

            if (theater != null)
            {
                return await Task.FromResult(new TheaterInfo() { 
                    Id = theater.Id,
                    Name = theater.Name,
                    Address = theater.Address,
                    Contact = theater.Contact,
                    Estado = theater.Estado,
                    Localization = new LocalizationInfo()
                    {
                        Id = theater.IdLocalizationNavigation.Id,
                        Name = theater.IdLocalizationNavigation.Loc,
                    }
                });
            }

            return await Task.FromResult(new TheaterInfo() { Id = -1 });

        }

        public override async Task<Confirmation> ChangeState(TheaterInfoState request, ServerCallContext context)
        {
            var theater = DBcontext.Theaters.Include(t => t.IdLocalizationNavigation).FirstOrDefault(t => t.Id.Equals(request.Id));

            if (theater != null)
            {
                theater.Estado = request.Estado;

                DBcontext.SaveChanges();

                return await Task.FromResult(new Confirmation() { Id = 1 });
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
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
                    Estado = theater.Estado,
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