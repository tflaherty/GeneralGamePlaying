using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace API.GGP.GGPInterfacesNS
{
    public static class PlayerManagerProtocolConstants
    {
        public static readonly string ReadyReplyString = "ready";
        public static readonly string NilReplyString = "nil";
        public static readonly string DoneReplyString = "done";

        public static readonly string InfoCommandString = "info";
        public static readonly string StartCommandString = "start";
        public static readonly string PlayCommandString = "play";
        public static readonly string StopCommandString = "stop";
        public static readonly string AbortCommandString = "abort";
    }

}
