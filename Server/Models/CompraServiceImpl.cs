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
                            Estado = 2,
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

        public override async Task GetPurchases(UserConnected request, IServerStreamWriter<PurchaseInfo> responseStream, ServerCallContext context)
        {
            var purchases = DBcontext.Purchases.Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation);
            PurchaseInfo p;

            foreach (var purchase in purchases)
            {
                p = new PurchaseInfo()
                {
                    Id = purchase.Id,
                    Reference = purchase.Reference,
                    DatePurchase = purchase.DatePurchase.Ticks,
                    CompraLugares = purchase.CompraLugares,
                    Estado = purchase.Estado,
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
        public override async Task<PurchaseInfo> GetPurchase(PurchaseInfo request, ServerCallContext context)
        {
            var purchases = DBcontext.Purchases.Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation).FirstOrDefault(s => s.Id.Equals(request.Id));
            
            if(purchases != null)
            {
                PurchaseInfo p = new PurchaseInfo()
                {
                    Id = purchases.Id,
                    Reference = purchases.Reference,
                    DatePurchase = purchases.DatePurchase.Ticks,
                    CompraLugares = purchases.CompraLugares,
                    Estado = purchases.Estado,
                    Session = new SessionInfo()
                    {
                        Id = purchases.IdSessionNavigation.Id,
                        SessionDate = purchases.IdSessionNavigation.SessionDate.Ticks,
                        StartHour = purchases.IdSessionNavigation.StartHour.Ticks,
                        EndHour = purchases.IdSessionNavigation.EndHour.Ticks,
                        AvaiablePlaces = purchases.IdSessionNavigation.AvaiablePlaces,
                        TotalPlaces = purchases.IdSessionNavigation.TotalPlaces,
                        Show = new ShowInfo()
                        {
                            Id = purchases.IdSessionNavigation.IdShowNavigation.Id,
                            Name = purchases.IdSessionNavigation.IdShowNavigation.Name,
                            Sinopse = purchases.IdSessionNavigation.IdShowNavigation.Sinopse,
                            StartDate = purchases.IdSessionNavigation.IdShowNavigation.StartDate.Ticks,
                            EndDate = purchases.IdSessionNavigation.IdShowNavigation.EndDate.Ticks,
                            Price = decimal.ToDouble(purchases.IdSessionNavigation.IdShowNavigation.Price),
                            Theater = new TheaterInfo()
                            {
                                Id = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Id,
                                Name = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Name,
                                Address = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Address,
                                Contact = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.Contact,
                                Localization = new LocalizationInfo()
                                {
                                    Id = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Id,
                                    Name = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Loc,
                                    Lat = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Lat,
                                    Longi = purchases.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation.Longi,
                                },
                            }
                        }
                    }
                };
                return await Task.FromResult(p);
            }
            
            return await Task.FromResult(new PurchaseInfo()
            {
                Id = -1,
            });
        }

        public override async Task<Confirmation> CancelPurchase(PurchaseInfo request, ServerCallContext context)
        {
            var purchases = DBcontext.Purchases.Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation).FirstOrDefault(s => s.Id.Equals(request.Id));

            if (purchases != null)
            {
                if((purchases.IdSessionNavigation.SessionDate.AddTicks(Math.Abs(purchases.IdSessionNavigation.EndHour.Ticks - purchases.IdSessionNavigation.StartHour.Ticks)) - DateTime.Now.Date).Ticks > new DateTime().AddDays(1).Ticks)
                {
                    purchases.Estado = 0;
                    decimal m = purchases.CompraLugares * purchases.IdSessionNavigation.IdShowNavigation.Price;
                    if (m <= purchases.IdUsersNavigation.Fundos)
                    {
                        purchases.IdUsersNavigation.Fundos -= m;
                    }
                }
            }

            return await Task.FromResult(new Confirmation()
            {
                Id = -1,
            }); ;
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
                    Estado = purchase.Estado,
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