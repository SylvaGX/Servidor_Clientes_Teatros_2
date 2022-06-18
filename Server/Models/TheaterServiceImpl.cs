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
            try
            {
                Theater theater = new()
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
            catch (DbUpdateException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'DbUpdateException': [{DateTime.Now}] - Error - Erro de atualizar a base de dados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar o teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<Confirmation> UpdateTheater(TheaterInfo request, ServerCallContext context)
        {
            try
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
                else
                {
                    return await Task.FromResult(new Confirmation() { Id = 0 });
                }
            }
            catch (DbUpdateException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'DbUpdateException': [{DateTime.Now}] - Error - Erro de atualizar a base de dados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao atualizar o teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task<TheaterInfo> GetTheater(TheaterInfo request, ServerCallContext context)
        {
            try
            {
                var theater = DBcontext.Theaters.Include(t => t.IdLocalizationNavigation).FirstOrDefault(t => t.Id.Equals(request.Id));

                if (theater != null)
                {
                    return await Task.FromResult(new TheaterInfo()
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
                        }
                    });
                }
                else
                {
                    return await Task.FromResult(new TheaterInfo() { Id = 0 });

                }
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao receber o teatro. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao atualizar o teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new TheaterInfo() { Id = -1 });

        }

        public override async Task<Confirmation> ChangeState(TheaterInfoState request, ServerCallContext context)
        {
            try
            {
                var theater = DBcontext.Theaters.Include(t => t.IdLocalizationNavigation).FirstOrDefault(t => t.Id.Equals(request.Id));

                if (theater != null)
                {
                    theater.Estado = request.Estado;

                    DBcontext.SaveChanges();

                    return await Task.FromResult(new Confirmation() { Id = 1 });
                }
                else
                {
                    return await Task.FromResult(new Confirmation() { Id = 0 });
                }
            }
            catch (DbUpdateException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'DbUpdateException': [{DateTime.Now}] - Error - Erro de atualizar a base de dados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao mudar o estado do teatro. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao mudar o estado do teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task GetTheaters(UserConnected request, IServerStreamWriter<TheaterInfo> responseStream, ServerCallContext context)
        {
            try
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
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao mudar o estado do teatro. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao mudar o estado do teatro.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
        }
    }
}