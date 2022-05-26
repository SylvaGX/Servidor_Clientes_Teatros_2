using Grpc.Core;
using gRPCProto;
using System;

namespace gRPCProto
{
    public static class gRPCProtoUtil
    {
        public static bool Exists(this UserConnected userConnected)
        {
            return userConnected != null && (userConnected.Id != -1);
        }
        public static bool Exists(this UserInfo userInfo)
        {
            return userInfo != null && (userInfo.Id != -1);
        }
        public static bool Exists(this UserRegister userInfo)
        {
            return userInfo != null && (!string.IsNullOrEmpty(userInfo.Email));
        }
        public static bool Exists(this ShowInfo showInfo)
        {
            return showInfo != null && (showInfo.Id != -1);
        }
        public static bool Exists(this Confirmation reserveConfirmation)
        {
            return reserveConfirmation != null && (reserveConfirmation.Id != -1);
        }
        public static bool Exists(this RefCompra refCompra)
        {
            return refCompra != null && (refCompra.Id != -1);
        }
        public static bool Exists(this TheaterInfo theaterInfo)
        {
            return theaterInfo != null && (theaterInfo.Id != -1);
        }
        public static bool Exists(this LocalizationInfo localizationInfo)
        {
            return localizationInfo != null && (localizationInfo.Id != -1);
        }

    }
}
