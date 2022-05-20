﻿// See https://aka.ms/new-console-template for more information

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

        public override async Task GetSessions(ShowInfo request, IServerStreamWriter<SessionInfo> responseStream, ServerCallContext context)
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
    }
}