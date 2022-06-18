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
            try
            {
                List<SessionInfoCompra> sessions = new List<SessionInfoCompra>();

                while (requestStream.MoveNext().Result)
                {
                    sessions.Add(requestStream.Current);
                }

                var user = DBcontext.Users.FirstOrDefault(u => u.Id.Equals(sessions.ElementAt(0).UserId));

                RefCompra refCompra = new RefCompra() { Id = -1, Reference = "" };

                if (user != null)
                {
                    decimal preco = Convert.ToDecimal(sessions.Sum(s => s.NumberPlaces * s.Price));
                    if (user.Fundos >= preco)
                    {

                        string r = new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString()+
                                   new Random().Next(0, 9).ToString();

                        Reference reference = new Reference()
                        {
                            Ref = r
                        };

                        DBcontext.References.Add(reference);

                        DBcontext.SaveChanges();

                        var re = DBcontext.References.OrderBy(p => p.Id).LastOrDefault();

                        if (re != null)
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
                                    Reference = re.Id,
                                };

                                DBcontext.Purchases.Add(purchase);
                            }

                            DBcontext.SaveChanges();

                            refCompra.Id = re.Id;
                            refCompra.PrecoTotal = decimal.ToDouble(preco);
                            refCompra.Reference = re.Ref;
                        }
                        else
                        {
                            refCompra.Id = -1;
                            refCompra.Reference = "";
                            LogServiceImpl logServiceImpl = new(DBcontext);

                            await logServiceImpl.LogWarning(new LogInfo()
                            {
                                Msg = $"'NotFoundDB': [{DateTime.Now}] - Error - Não foi encontrado a referencia na BD.",
                                LevelLog = 2
                            }, context);
                        }

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
                    LogServiceImpl logServiceImpl = new(DBcontext);

                    await logServiceImpl.LogWarning(new LogInfo()
                    {
                        Msg = $"'NotFoundDB': [{DateTime.Now}] - Error - Não foi encontrado o utlizador na BD com o id {sessions.ElementAt(0).UserId}.",
                        LevelLog = 2
                    }, context);
                }

                return await Task.FromResult(refCompra);
            }
            catch (DbUpdateException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao atualizar a base de dados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (OverflowException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'OverflowException': [{DateTime.Now}] - Error - Erro de overflow ao converter o dinheiro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao comprar a sessão. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao comprar a sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new RefCompra()
            {
                Id = -1,
                Reference = ""
            });
        }

        public override async Task GetPurchases(UserConnected request, IServerStreamWriter<PurchaseInfo> responseStream, ServerCallContext context)
        {
            try
            {
                var purchases = DBcontext.Purchases.Include(p => p.ReferenceNavigation).Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation);
                PurchaseInfo p;

                foreach (var purchase in purchases)
                {
                    p = new PurchaseInfo()
                    {
                        Id = purchase.Id,
                        Reference = new ReferenceInfo(){
                            Id = purchase.ReferenceNavigation.Id,
                            Ref = purchase.ReferenceNavigation.Ref
                        },
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
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber as sessões. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as sessões.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
        }

        public async override Task<Confirmation> PayPurchases(RefCompra request, ServerCallContext context)
        {
            try
            {
                var purchases = DBcontext.Purchases.Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation).Where(s => s.Reference.Equals(request.Id) && s.Estado.Equals(2));

                if (purchases != null && purchases.Count() > 0)
                {
                    var user = DBcontext.Users.FirstOrDefault(u => u.Id.Equals(purchases.First().IdUsers));

                    if(user != null)
                    {

                        foreach (var purchase in purchases)
                        {
                            purchase.Estado = 1;
                        }

                        user.Fundos -= Convert.ToDecimal(request.PrecoTotal);

                        DBcontext.SaveChanges();

                        return await Task.FromResult(new Confirmation() { Id = 1 });
                    }
                    else
                    {
                        LogServiceImpl logServiceImpl = new(DBcontext);

                        await logServiceImpl.LogError(new LogInfo()
                        {
                            Msg = $"'DbNotFound': [{DateTime.Now}] - Error - User não encontrado.",
                            LevelLog = 2
                        }, context);

                        return await Task.FromResult(new Confirmation()
                        {
                            Id = -1,
                        });
                    }

                }
                else
                {
                    LogServiceImpl logServiceImpl = new(DBcontext);

                    await logServiceImpl.LogError(new LogInfo()
                    {
                        Msg = $"'DbNotFound': [{DateTime.Now}] - Error - Compras não encontradas.",
                        LevelLog = 2
                    }, context);

                    return await Task.FromResult(new Confirmation()
                    {
                        Id = -1,
                    });
                }
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao pagar as sessões. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao pagar as sessões.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation()
            {
                Id = -1,
            });
        }

        public override async Task<PurchaseInfo> GetPurchase(PurchaseInfo request, ServerCallContext context)
        {
            try
            {
                var purchases = DBcontext.Purchases.Include(p => p.IdUsersNavigation).Include(p => p.ReferenceNavigation).Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation).FirstOrDefault(s => s.Id.Equals(request.Id));

                if (purchases != null)
                {
                    PurchaseInfo p = new PurchaseInfo()
                    {
                        Id = purchases.Id,
                        Reference = new ReferenceInfo()
                        {
                            Id = purchases.ReferenceNavigation.Id,
                            Ref = purchases.ReferenceNavigation.Ref
                        },
                        DatePurchase = purchases.DatePurchase.Ticks,
                        CompraLugares = purchases.CompraLugares,
                        Estado = purchases.Estado,
                        User = new UserInfo()
                        {
                            Id = purchases.IdUsersNavigation.Id,
                        },
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
                else
                {
                    return await Task.FromResult(new PurchaseInfo()
                    {
                        Id = -1,
                    });
                }
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber a sessão. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber a sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new PurchaseInfo()
            {
                Id = -1,
            });
        }

        public override async Task<Confirmation> CancelPurchase(PurchaseInfo request, ServerCallContext context)
        {
            try
            {
                var purchases = DBcontext.Purchases.Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation).FirstOrDefault(s => s.Id.Equals(request.Id));

                if (purchases != null)
                {
                    if ((purchases.IdSessionNavigation.SessionDate.AddTicks(Math.Abs(purchases.IdSessionNavigation.EndHour.Ticks - purchases.IdSessionNavigation.StartHour.Ticks)) - DateTime.Now.Date).Ticks > new DateTime().AddDays(1).Ticks)
                    {
                        purchases.Estado = 0;
                        decimal m = purchases.CompraLugares * purchases.IdSessionNavigation.IdShowNavigation.Price;
                        var user = DBcontext.Users.FirstOrDefault(u => u.Id.Equals(request.User.Id));
                        if (m <= purchases.IdUsersNavigation.Fundos && user != null)
                        {
                            purchases.IdUsersNavigation.Fundos -= m;
                            purchases.IdSessionNavigation.AvaiablePlaces -= purchases.CompraLugares;
                            user.Fundos += m;

                            DBcontext.SaveChanges();

                            return await Task.FromResult(new Confirmation()
                            {
                                Id = 1
                            });
                        }
                    }
                }
                else
                {
                    return await Task.FromResult(new Confirmation()
                    {
                        Id = -1,
                    });
                }
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao cancelar a compra. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (OverflowException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'OverflowException': [{DateTime.Now}] - Error - Erro de overflow ao comparar as datas da sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro de fora do alcance ao comparar as datas da sessão. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
             catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao cancelar a compra.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation()
            {
                Id = -1,
            });
        }

        public override async Task HistoryUser(UserConnected request, IServerStreamWriter<PurchaseInfo> responseStream, ServerCallContext context)
        {
            try
            {
                var purchases = DBcontext.Purchases.Include(p => p.ReferenceNavigation).Include(p => p.IdSessionNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation).Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation)
                            .Include(p => p.IdSessionNavigation.IdShowNavigation.IdTheaterNavigation.IdLocalizationNavigation).Where(s => s.IdUsers.Equals(request.Id));
                PurchaseInfo p;

                foreach (var purchase in purchases)
                {
                    p = new PurchaseInfo()
                    {
                        Id = purchase.Id,
                        Reference = new ReferenceInfo()
                        {
                            Id = purchase.ReferenceNavigation.Id,
                            Ref = purchase.ReferenceNavigation.Ref
                        },
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
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber a história de compras. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber a história de compras.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
        }
    }
}