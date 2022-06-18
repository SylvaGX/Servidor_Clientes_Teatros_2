// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;
using Microsoft.EntityFrameworkCore;

namespace Server.Models
{
    internal class LocalizationServiceImpl : LocalizationService.LocalizationServiceBase
    {
        private ServerContext DBcontext;
        
        public LocalizationServiceImpl(ServerContext context)
        {
            this.DBcontext = context;
        }

        public override async Task<Confirmation> AddLocalization(LocalizationInfo request, ServerCallContext context)
        {
            try
            {
                Localization localization = new Localization()
                {
                    Loc = request.Name,
                    Lat = request.Lat,
                    Longi = request.Longi,
                };

                DBcontext.Add(localization);

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
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao adicionar a localização.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }

            return await Task.FromResult(new Confirmation() { Id = -1 });
        }

        public override async Task GetLocalizations(UserConnected request, IServerStreamWriter<LocalizationInfo> responseStream, ServerCallContext context)
        {
            try
            {
                var localizations = DBcontext.Localizations;
                LocalizationInfo l;

                foreach (var localization in localizations)
                {
                    l = new LocalizationInfo()
                    {
                        Id = localization.Id,
                        Name = localization.Loc,
                        Lat = localization.Lat,
                        Longi = localization.Longi,
                    };

                    await responseStream.WriteAsync(l);
                }
            }
            catch (Exception ex)
            {
                LogServiceImpl logServiceImpl = new(DBcontext);

                await logServiceImpl.LogError(new LogInfo()
                {
                    Msg = $"'Exception': [{DateTime.Now}] - Error - Erro ao receber as localizações.\nCode Msg: {ex.Message}",
                    LevelLog = 3
                }, context);
            }
        }
    }
}