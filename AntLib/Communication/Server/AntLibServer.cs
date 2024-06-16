using AntLib.Communication.Client;
using AntLib.Model;
using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication.Server
{
    public class AntLibServer
    {
        private ServerCommandHandler _handler;
        private ServerConnection _connection;
        public Action<string> OnMessageReceive;

        public AntLibServer(string serverName, string adress, int port)
        {
            _handler = new ServerCommandHandler();
            _connection = new ServerConnection(serverName, adress, port);
            _connection.SubscribeOnMessageRecieve(HandleIncomingMessage);
            _connection.SubscribeOnMessageRecieve(_handler.HandleCommand);
            _handler.SubscribeOnEndHandle(_connection.SendMessage);
        }

        public void SetModel(ModelBuilder modelBuilder, float trainSpeed)
        {
            _handler.SetModel(modelBuilder, trainSpeed);
        }

        public void SetTrainData(DataArray[] xTrain, DataArray[] yTrain, DataArray[] xTest, DataArray[] yTest)
        {
            _handler.SetTrainData(xTrain, yTrain, xTest, yTest);
        }

        public void Fit(int epoch)
        {
            _handler.Fit(epoch);
        }

        public ILayerInfo[] GetModelInfo()
        {
            return _handler.GetLayerInfos();
        }

        internal void HandleIncomingMessage(Message message)
        {
            string clientInfo = $"client {message.SenderName}: ";
            switch(message.Command)
            {
                case Command.ConnectionReady:
                    clientInfo += "connected";
                    break;
                case Command.TakeFitInfo:
                    clientInfo += $"{((FitInfo)Serializer.Deserialize(message.Data)).ToString()}";
                    break;

                case Command.ModelBuilded:
                    clientInfo += "model ready to fit";
                    break;

                default:
                    clientInfo += Enum.GetName(message.Command.GetType(), message.Command);
                    break;
            }

            OnMessageReceive?.Invoke(clientInfo);
        }
    }
}
