using AntLib.Communication.Server;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;

namespace AntLib.Communication.Client
{
    internal class ClientConnection
    {
        private string _hostName;
        private Action<Message> _onMessageRecieve;
        private string _adress;
        private int _port;
        WatsonTcpClient _client;
        internal ClientConnection(string hostName)
        {
            _hostName = hostName;
        }

        internal void ConnectToServer(string adress, int port)
        {
            _adress = adress;
            _port = port;
            _client = new WatsonTcpClient(adress, port);
            _client.Events.MessageReceived += MessageReceived;
            _client.Connect();
        }
        internal void SubscribeOnMessageRecieve(Action<Message> action)
        {
            _onMessageRecieve += action;
        }

        internal void UnscribeOnMessageRecieve(Action<Message> action)
        {
            _onMessageRecieve -= action;
        }

        internal void SendMessage(Message msg)
        {
            msg.SenderName = _hostName;
            _client.SendAsync(msg.ToJson()).Wait();
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Message msg = (Message)Serializer.Deserialize(Encoding.UTF8.GetString(args.Data));
            _onMessageRecieve(msg);
        }
    }
}
