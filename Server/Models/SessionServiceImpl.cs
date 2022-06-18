// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class SessionServiceImpl : SessionService.SessionServiceBase
    {
        private ServerContext DBcontext;
        
        public SessionServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override async Task<Confirmation> AddSession(SessionInfo request, ServerCallContext context)
        {
            try
            {
                Session session = new Session()
                {
                    SessionDate = new DateTime(request.SessionDate),
                    StartHour = new TimeSpan(request.StartHour),
                    EndHour = new TimeSpan(request.EndHour),
                    AvaiablePlaces = 0,
                    TotalPlaces = request.TotalPlaces,
                    IdShow = request.Show.Id,
                    Estado = 1,
                };

                DBcontext.Add(session);

                DBcontext.SaveChanges();

                return await Task.FromResult(new Confirmation() { Id = 1 });
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao adicionar a sessão. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar a sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<Confirmation> UpdateSession(SessionInfo request, ServerCallContext context)
        {
            try
            {
                var session = DBcontext.Sessions.FirstOrDefault(x => x.Id == request.Id);

                if (session != null)
                {
                    session.SessionDate = new DateTime(request.SessionDate);
                    session.StartHour = new TimeSpan(request.StartHour);
                    session.EndHour = new TimeSpan(request.EndHour);
                    session.TotalPlaces = request.TotalPlaces;
                    session.IdShow = request.Show.Id;
                    session.Estado = request.Estado;

                    DBcontext.Update(session);

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
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao atualizar a sessão. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao atualizar a sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<Confirmation> ChangeState(SessionInfoState request, ServerCallContext context)
        {
            try
            {
                var session = DBcontext.Sessions.Include(t => t.IdShowNavigation).FirstOrDefault(t => t.Id.Equals(request.Id));

                if (session != null)
                {
                    session.Estado = request.Estado;

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
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao mudar o estado da sessão. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao mudar o estado da sessão.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task GetAllSessions(UserConnected request, IServerStreamWriter<SessionInfo> responseStream, ServerCallContext context)
        {
            try
            {
                var sessions = DBcontext.Sessions.Include(s => s.IdShowNavigation);
                SessionInfo s;

                foreach (var session in sessions)
                {
                    s = new SessionInfo()
                    {
                        Id = session.Id,
                        SessionDate = session.SessionDate.Ticks,
                        StartHour = session.StartHour.Ticks,
                        EndHour = session.EndHour.Ticks,
                        AvaiablePlaces = session.AvaiablePlaces,
                        TotalPlaces = session.TotalPlaces,
                        Estado = session.Estado,
                        Show = new ShowInfo()
                        {
                            Id = session.IdShowNavigation.Id,
                            Name = session.IdShowNavigation.Name,
                            StartDate = session.IdShowNavigation.StartDate.Ticks,
                            EndDate = session.IdShowNavigation.EndDate.Ticks,
                            Sinopse = session.IdShowNavigation.Sinopse,
                            Price = decimal.ToDouble(session.IdShowNavigation.Price),
                            Estado = session.IdShowNavigation.Estado,
                        },
                    };

                    await responseStream.WriteAsync(s);
                }
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber todas as sessões. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber todas as sessões.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
        }

        public override async Task GetSessions(ShowInfo request, IServerStreamWriter<SessionInfo> responseStream, ServerCallContext context)
        {
            try
            {
                var sessions = DBcontext.Sessions.Include(s => s.IdShowNavigation).Where(s => s.IdShowNavigation.Id.Equals(request.Id));
                SessionInfo s;

                foreach (var session in sessions)
                {
                    s = new SessionInfo()
                    {
                        Id = session.Id,
                        SessionDate = session.SessionDate.Ticks,
                        StartHour = session.StartHour.Ticks,
                        EndHour = session.EndHour.Ticks,
                        AvaiablePlaces = session.AvaiablePlaces,
                        TotalPlaces = session.TotalPlaces,
                    };

                    await responseStream.WriteAsync(s);
                }
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber as sessões de um show. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as sessões de um show.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
        }
    }
}