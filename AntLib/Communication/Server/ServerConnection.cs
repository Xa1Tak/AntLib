using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WatsonTcp;

namespace AntLib.Communication.Server
{
    internal class ServerConnection
    {
        private string _hostName;
        private Action<Message> _onMessageRecieve;
        WatsonTcpServer _server;
        internal ServerConnection(string hostName, string adress, int port)
        {
            _hostName = hostName;
            _server = new WatsonTcpServer(adress, port);
            _server.Events.MessageReceived += MessageReceived;
            _server.Start();
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
            if(msg.ReceiverName == null)
            {
                SendToAll(msg);
                return;
            }
            _server.SendAsync(ClientAgregator.GetClientGuid(msg.ReceiverName), msg.ToJson());
        }

        internal void SendToAll(Message msg)
        {
            string[] clients = ClientAgregator.GetClientsName();
            foreach(string client in clients)
            {
                msg.ReceiverName = client;
                _server.SendAsync(ClientAgregator.GetClientGuid(client), msg.ToJson()).Wait();
            }
        }

        private void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Message msg = (Message)Serializer.Deserialize(Encoding.UTF8.GetString(args.Data));
            if(msg.Command == Command.ConnectionReady)
            {
                ClientAgregator.AddClient(msg.SenderName, args.Client.Guid, msg.Command);
            }
            ClientAgregator.ChangeClientState(msg.SenderName, msg.Command);
            _onMessageRecieve?.Invoke(msg);
        }
    }
}
