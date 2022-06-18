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
            try
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
            catch (DbUpdateException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'DbUpdateException': [{DateTime.Now}] - Error - Erro ao atualizar a base de dados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar o show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<Confirmation> UpdateShow(ShowInfo request, ServerCallContext context)
        {
            try
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
                else
                {
                    return await Task.FromResult(new Confirmation() { Id = -1 });
                }
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
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao atualizar o show. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao atualizar o show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<ShowInfo> GetShow(ShowInfo request, ServerCallContext context)
        {
            try
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
                else
                {
                    return await Task.FromResult(new ShowInfo() { Id = -1 });
                }
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber o show. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber o show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new ShowInfo() { Id = -1 });
        }

        public override async Task<Confirmation> ChangeState(ShowInfoState request, ServerCallContext context)
        {
            try
            {
                var show = DBcontext.Shows.Include(t => t.IdTheaterNavigation).FirstOrDefault(s => s.Id.Equals(request.Id));

                if (show != null)
                {
                    show.Estado = request.Estado;

                    DBcontext.SaveChanges();

                    return await Task.FromResult(new Confirmation() { Id = 1 });
                }
                else
                {
                    return await Task.FromResult(new Confirmation() { Id = -1 });
                }
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
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao mudar o estado do show. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao muda o estado do show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task GetShows(UserConnected request, IServerStreamWriter<ShowInfo> responseStream, ServerCallContext context)
        {
            try
            {
                var shows = DBcontext.Shows.Include(s => s.IdTheaterNavigation).Include(s => s.IdTheaterNavigation.IdLocalizationNavigation);
                ShowInfo s;
                List<Session> sessions = new();

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
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber os shows. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber os shows.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

        }
    }
}