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
            Confirmation confirmation = new() { Id = 1 };

            Session? session = DBcontext.Sessions.FirstOrDefault(s => s.Id.Equals(request.Id));

            if (session != null)
            {
                if(request.NumberPlaces <= (session.TotalPlaces - session.AvaiablePlaces))
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

        public override async Task<Confirmation> CancelReservationPlaces(SessionInfoReserve request, ServerCallContext context)
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
    }
}