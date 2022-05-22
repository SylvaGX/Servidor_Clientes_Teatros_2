// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class CompraServiceImpl : CompraService.CompraServiceBase
    {
        private ServerContext DBcontext;
        
        public CompraServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override async Task<RefCompra> BuySessions(IAsyncStreamReader<SessionInfoCompra> requestStream, ServerCallContext context)
        {
            List<SessionInfoCompra> sessions = new List<SessionInfoCompra>();

            while (requestStream.MoveNext().Result)
            {
                sessions.Add(requestStream.Current);
            }

            var user = DBcontext.Users.FirstOrDefault(u => u.Id.Equals(sessions.ElementAt(0).UserId));

            RefCompra refCompra = new RefCompra() { Id = 1, Reference = "123456789" };

            if (user != null)
            {
                if(user.Fundos >= Convert.ToDecimal(sessions.Sum(s => s.NumberPlaces * s.Price)))
                {
                    Purchase purchase;

                    foreach (var session in sessions)
                    {
                        purchase = new Purchase()
                        {
                            IdSession = session.Id,
                            IdUsers = session.UserId,
                            CompraLugares = session.NumberPlaces,
                            Reference = "123456789",
                        };

                        DBcontext.Purchases.Add(purchase);
                    }

                    user.Fundos -= Convert.ToDecimal(sessions.Sum(s => s.NumberPlaces * s.Price));

                    DBcontext.SaveChanges();
                }
                else
                {
                    refCompra.Id = 0;
                    refCompra.Reference = "";
                }
            }
            else
            {
                refCompra.Id = -1;
                refCompra.Reference = "";

            }

            return await Task.FromResult(refCompra);
        }

        public override async Task HistoryUser(UserConnected request, IServerStreamWriter<PurchaseInfo> responseStream, ServerCallContext context)
        {

            var purchases = DBcontext.Purchases.Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation).Where(s => s.IdUsers.Equals(request.Id));
            PurchaseInfo p;

            foreach (var purchase in purchases)
            {
                p = new PurchaseInfo()
                {
                    Id = purchase.Id,
                    Reference = purchase.Reference,
                    DatePurchase = purchase.DatePurchase.Ticks,
                    CompraLugares = purchase.CompraLugares,
                    Session = new SessionInfo()
                    {
                        Id = purchase.IdSessionNavigation.Id,
                        SessionDate = purchase.IdSessionNavigation.SessionDate.Ticks,
                        StartHour = purchase.IdSessionNavigation.StartHour.Ticks,
                        EndHour = purchase.IdSessionNavigation.EndHour.Ticks,
                        AvaiablePlaces = purchase.IdSessionNavigation.AvaiablePlaces,
                        TotalPlaces = purchase.IdSessionNavigation.TotalPlaces,
                        Show = new ShowInfo()
                        {
                            Id = purchase.IdSessionNavigation.IdShowNavigation.Id,
                            Name = purchase.IdSessionNavigation.IdShowNavigation.Name,
                            Sinopse = purchase.IdSessionNavigation.IdShowNavigation.Sinopse,
                            StartDate = purchase.IdSessionNavigation.IdShowNavigation.StartDate.Ticks,
                            EndDate = purchase.IdSessionNavigation.IdShowNavigation.EndDate.Ticks,
                            Price = decimal.ToDouble(purchase.IdSessionNavigation.IdShowNavigation.Price),
                            Theater = new TheaterInfo()
                            {
                                Id = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Id,
                                Name = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Name,
                                Address = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Address,
                                Contact = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Contact,
                                Localization = new LocalizationInfo()
                                {
                                    Id = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Id,
                                    Name = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Loc,
                                    Lat = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Lat,
                                    Longi = purchase.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Longi,
                                },
                            }
                        }
                    }
                };

                await responseStream.WriteAsync(p);
            }
        }
    }
}