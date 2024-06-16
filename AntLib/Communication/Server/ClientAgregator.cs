using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication.Server
{
    internal static class ClientAgregator
    {
        private static Dictionary<string, ClientInfo> _clients = new Dictionary<string, ClientInfo>();

        internal static void AddClient(string name, Guid adress, Command state)
        {
            if (!_clients.ContainsKey(name))
            {
                _clients.Add(name, new ClientInfo(name, adress, state));
            }
        }

        internal static void RemoveClient(string name)
        {
            if (_clients.ContainsKey(name))
            {
                _clients.Remove(name);
            }
        }

        internal static void ChangeClientState(string name, Command command)
        {
            if(_clients.ContainsKey(name))
            {
                _clients[name].ChangeState(command);
            }
        }

        internal static string[] GetClientsName()
        {
            return _clients.Keys.ToArray();
        }

        internal static string[] GetClientsNameByState(Command state)
        {
            return _clients.Values.Where(x => x.State == state).Select(x => x.Name).ToArray();
        }

        internal static ClientInfo[] GetClientInfos()
        {
            return _clients.Values.ToArray();
        }

        internal static Guid GetClientGuid(string name)
        {
            if (_clients.ContainsKey(name))
            {
                return _clients[name].Adress;
            }
            return Guid.Empty;
        }

        internal static bool IsClientWithState(Command state)
        {
            return _clients.Values.Where(x => x.State == state).Count() > 0;
        }

        internal static bool ClientStateIs(Command state)
        {
            return _clients.Values.Where(x => x.State == state).Count() == _clients.Count;
        }

        internal static int CLientsCount()
        {
            return _clients.Count;
        }
    }
}
