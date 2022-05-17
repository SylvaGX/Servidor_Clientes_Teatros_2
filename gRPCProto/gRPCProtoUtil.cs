using Grpc.Core;
using GRPCProto;
using System;

namespace GRPCProto
{
    public static class gRPCProtoUtil
    {
        public static bool Exists(this UserConnected userConnected)
        {
            return userConnected != null && (userConnected.Id != -1);
        }
    }
}
