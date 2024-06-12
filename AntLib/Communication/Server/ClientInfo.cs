using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication.Server
{
    internal struct ClientInfo
    {
        public string Name;
        public Command State;
        public Guid Adress;

        public ClientInfo(string name, Guid adress, Command state)
        {
            Name = name;
            Adress = adress;
            State = state;
        }

        public void ChangeState(Command command)
        {
            State = command;
        }
    }
}
