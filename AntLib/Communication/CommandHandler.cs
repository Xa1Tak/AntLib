using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication
{
    internal class CommandHandler
    {
        protected Action<Message> _action;

        internal virtual void HandleCommand(Message msg)
        {
            throw new NotImplementedException();
        }

        internal void SubscribeOnEndHandle(Action<Message> action)
        {
            _action += action;
        }

        internal void UnscribeOnEndHandle(Action<Message> action)
        {
            _action -= action;
        }

        protected void SendAnswerMessage(Command command, string data)
        {
            Message message = new Message();
            message.Data = data;
            message.Command = command;
            _action.Invoke(message);
        }
    }
}
