using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication
{
    public struct Message
    {
        public string SenderName;
        public string ReceiverName;
        public Command Command;
        public string Data;


        internal Message(string senderName, string receiverName, Command command, string data)
        {
            SenderName = senderName;
            ReceiverName = receiverName;
            Command = command; 
            Data = data;
        }

        internal string ToJson()
        {
            return Serializer.Serialize(this);
        }
    }
}
