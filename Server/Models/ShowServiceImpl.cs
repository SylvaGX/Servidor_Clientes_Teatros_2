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