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

    }
}
