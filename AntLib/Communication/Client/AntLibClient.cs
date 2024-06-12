using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication.Client
{
    public class AntLibClient
    {
        private ClientCommandHandler _handler;
        private ClientConnection _connection;
        public Action<Message> OnMessageReceive;

        public AntLibClient(string clientnName, bool isParallel, int threads)
        {
            _handler = new ClientCommandHandler(clientnName);
            _connection = new ClientConnection(clientnName);
            _connection.SubscribeOnMessageRecieve(_handler.HandleCommand);
            _handler.SubscribeOnEndHandle(_connection.SendMessage);
            _handler.ChooseModel(isParallel, threads);
        }

        public void ConnectToServer(string adress, int ip)
        {
            _connection.SubscribeOnMessageRecieve(OnMessageReceive);
            _connection.ConnectToServer(adress, ip);
            _handler.SayReady();
        }
    }
}
