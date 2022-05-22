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