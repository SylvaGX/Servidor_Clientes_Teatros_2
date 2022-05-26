// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class ShowServiceImpl : ShowService.ShowServiceBase
    {
        private ServerContext DBcontext;
        
        public ShowServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override async Task<Confirmation> AddShow(ShowInfo request, ServerCallContext context)
        {
            Show show = new Show()
            {
                Name = request.Name,
                Sinopse = request.Sinopse,
                StartDate = new DateTime(request.StartDate),
                EndDate = new DateTime(request.EndDate),
                Price = Convert.ToDecimal(request.Price),
                Estado = 1,
                IdTheater = request.Theater.Id,
            };

            DBcontext.Add(show);

            DBcontext.SaveChanges();

            return await Task.FromResult(new Confirmation() { Id = 1 });
        }

        public override async Task<Confirmation> UpdateShow(ShowInfo request, ServerCallContext context)
        {
            var show = DBcontext.Shows.FirstOrDefault(x => x.Id == request.Id);

            if (show != null)
            {
                show.Name = request.Name;
                show.Sinopse = request.Sinopse;
                show.StartDate = new DateTime(request.StartDate);
                show.EndDate = new DateTime(request.EndDate);
                show.Price = Convert.ToDecimal(request.Price);
                show.IdTheater = request.Theater.Id;
                show.Estado = request.Estado;

                DBcontext.Update(show);

                DBcontext.SaveChanges();

                return await Task.FromResult(new Confirmation() { Id = 1 });
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<ShowInfo> GetShow(ShowInfo request, ServerCallContext context)
        {
            var show = DBcontext.Shows.Include(t => t.IdTheaterNavigation).FirstOrDefault(s => s.Id.Equals(request.Id));

            if (show != null)
            {
                return await Task.FromResult(new ShowInfo()
                {
                    Id = show.Id,
                    Name = show.Name,
                    Sinopse = show.Sinopse,
                    StartDate = show.StartDate.Ticks,
                    EndDate = show.EndDate.Ticks,
                    Price = decimal.ToDouble(show.Price),
                    Estado = show.Estado,
                    Theater = new TheaterInfo()
                    {
                        Id = show.IdTheaterNavigation.Id,
                        Name = show.IdTheaterNavigation.Name,
                    }
                });
            }

            return await Task.FromResult(new ShowInfo() { Id = -1 });
        }

        public override async Task<Confirmation> ChangeState(ShowInfoState request, ServerCallContext context)
        {
            var show = DBcontext.Shows.Include(t => t.IdTheaterNavigation).FirstOrDefault(s => s.Id.Equals(request.Id));

            if (show != null)
            {
                show.Estado = request.Estado;

                DBcontext.SaveChanges();

                return await Task.FromResult(new Confirmation() { Id = 1 });
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task GetShows(UserConnected request, IServerStreamWriter<ShowInfo> responseStream, ServerCallContext context)
        {
            var shows = DBcontext.Shows.Include(s => s.IdTheaterNavigation).Include(s => s.IdTheaterNavigation.IdLocalizationNavigation);
            ShowInfo s;
            List<Session> sessions = new List<Session>();

            foreach (var show in shows)
            {
                sessions.AddRange(DBcontext.Sessions.Where(s => show.Sessions.Contains(s)).AsEnumerable());
                s = new ShowInfo()
                {
                    Id = show.Id,
                    Name = show.Name,
                    Sinopse = show.Sinopse,
                    StartDate = show.StartDate.Ticks,
                    EndDate = show.EndDate.Ticks,
                    Price = decimal.ToDouble(show.Price),
                    Estado = show.Estado,
                    Theater = new TheaterInfo()
                    {
                        Id = show.IdTheaterNavigation.Id,
                        Name = show.IdTheaterNavigation.Name,
                        Address = show.IdTheaterNavigation.Address,
                        Contact = show.IdTheaterNavigation.Contact,
                        Localization = new LocalizationInfo()
                        {
                            Id = show.IdTheaterNavigation.IdLocalizationNavigation.Id,
                            Name = show.IdTheaterNavigation.IdLocalizationNavigation.Loc,
                            Lat = show.IdTheaterNavigation.IdLocalizationNavigation.Lat,
                            Longi = show.IdTheaterNavigation.IdLocalizationNavigation.Longi,
                        },
                    }
                };

                await responseStream.WriteAsync(s);
            }
        }
    }
}