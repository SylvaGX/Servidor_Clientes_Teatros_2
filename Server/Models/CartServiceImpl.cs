// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class CartServiceImpl : CartService.CartServiceBase
    {
        private ServerContext DBcontext;
        
        public CartServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override async Task<Confirmation> ReservePlaces(SessionInfoReserve request, ServerCallContext context)
        {
            try
            {
                Confirmation confirmation = new() { Id = 1 };

                Session? session = DBcontext.Sessions.FirstOrDefault(s => s.Id.Equals(request.Id));

                if (session != null)
                {
                    if (request.NumberPlaces <= (session.TotalPlaces - session.AvaiablePlaces))
                    {
                        session.AvaiablePlaces += request.NumberPlaces;

                        DBcontext.SaveChanges();
                    }
                    else
                    {
                        confirmation.Id = 0;
                    }
                }
                else
                {
                    confirmation.Id = -1;
                }

                return await Task.FromResult(confirmation);
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
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao reservar lugares. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao reservar lugares.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation()
            {
                Id = -1,
            });
        }

        public override async Task<Confirmation> CancelReservationPlaces(SessionInfoReserve request, ServerCallContext context)
        {
            try
            {
                Confirmation confirmation = new() { Id = 1 };

                Session? session = DBcontext.Sessions.FirstOrDefault(s => s.Id.Equals(request.Id));

                if (session != null)
                {
                    session.AvaiablePlaces -= request.NumberPlaces;

                    DBcontext.SaveChanges();
                }
                else
                {
                    confirmation.Id = -1;
                }

                return await Task.FromResult(confirmation);
            }
            catch (ArgumentNullException ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'ArgumentNullException': [{DateTime.Now}] - Error - Erro ao cancelar lugares reservados. Argumento nulo.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao cancelar lugares reservados.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation()
            {
                Id = -1,
            });
        }
    }
}