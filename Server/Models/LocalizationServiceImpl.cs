// See https://aka.ms/new-console-template for more information

using Grpc.Core;
using Server.Data;
using gRPCProto;

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

        public override async Task GetLocalizations(UserConnected request, IServerStreamWriter<LocalizationInfo> responseStream, ServerCallContext context)
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
    }
}