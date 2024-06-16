using AntLib.Model;
using AntLib.Model.Layer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication.Server
{
    internal class ServerCommandHandler : CommandHandler
    {
        internal int ClientCount { get; set; }
        private int _epochToEndFit;
        private int _curEpoch;
        private List<ILayerInfo[]> _modelsLayerInfo;

        private ModelBuilder _modelBuilder;

        private Command _fitCommand;
        internal bool IsFiting { get; private set; }
        private float _trainSpeed;

        private DataArray[] _xTrain, _yTrain, _xTest, _yTest;

        private int _fixClientCounter = 0;



        public ServerCommandHandler() 
        {
            _modelsLayerInfo = new List<ILayerInfo[]>();
        }

        public ILayerInfo[] GetLayerInfos()
        {
            return _modelBuilder.GetLayerInfo();
        }

        internal override void HandleCommand(Message msg)
        {
            Object obj = Serializer.Deserialize(msg.Data);
            switch(msg.Command)
            {
                case Command.ConnectionReady:
                    OnConnectionReady(msg);
                    break;

                case Command.FitEnded:
                    OnFitEnded(msg);
                    break;

                case Command.UpdateInfoEnded:
                    OnUpdateInfoEnded(msg);
                    break;

                case Command.TakeFitInfo:

                    break;

                case Command.TakeLayerInfo:
                    OnTakeLayerInfo(msg);
                    break;

                case Command.ModelBuilded:
                    OnModelBuilded(msg);
                    break;
            }
        }

        internal void Fit(int epochCount)
        {
            _curEpoch = 0;
            _epochToEndFit = epochCount;
            Fit();
        }

        private void Fit()
        {
            if (_curEpoch == _epochToEndFit)
            {
                IsFiting = false;
                return;
            }
            IsFiting = true;
            SendTrainData();
            SendAnswerMessage(_fitCommand, Serializer.Serialize(_curEpoch));
            _curEpoch++;
        }

        internal void SetModel(ModelBuilder modelBuilder, float trainSpeed)
        {
            if (_modelBuilder == null) _modelBuilder = modelBuilder;
            _trainSpeed = trainSpeed;
            //SendAnswerMessage(Command.SetAccuracy, Serializer.Serialize(_modelBuilder.GetAccuracy));
            //SendAnswerMessage(Command.SetLoss, Serializer.Serialize(_modelBuilder.GetLoss));
            //SendAnswerMessage(Command.SetFitOptimizer, Serializer.Serialize(_modelBuilder.GetOptimizerParam));
            //SendAnswerMessage(Command.SetLayers, Serializer.Serialize(_modelBuilder.GetLayerInfo));
            //SendAnswerMessage(Command.SetTrainSpeed, Serializer.Serialize(trainSpeed));
        }

        internal void SetTrainData(DataArray[] xTrain, DataArray[] yTrain, DataArray[] xTest, DataArray[] yTest)
        {
            _xTrain = xTrain;
            _yTrain = yTrain;
            _xTest = xTest;
            _yTest = yTest;
        }

        internal void SendTrainData()
        {
            var splitedTrain = ParallelModelHelper.SplitData(_xTrain, _yTrain, ClientAgregator.CLientsCount());
            SendTrainData(Command.SetTrainDataX, splitedTrain.X);
            SendTrainData(Command.SetTrainDataY, splitedTrain.Y);
            if (_xTest == null || _yTest == null)
            {
                _fitCommand = Command.FitWithoutEval;
                return;
            }
            var splitedTest = ParallelModelHelper.SplitData(_xTest, _yTest, ClientAgregator.CLientsCount());
            SendTrainData(Command.SetTestDataX, splitedTest.X);
            SendTrainData(Command.SetTestDataY, splitedTest.Y);
            _fitCommand = Command.FitWithEval;
        }

        private void SendTrainData(Command command, DataArray[][] data)
        {
            string[] clients = ClientAgregator.GetClientsName();
            for (int i = 0; i < clients.Length; i++)
            {
                SendMessageTo(command, clients[i], Serializer.Serialize(data[i]));
            }
        }

        private void SendMessageTo(Command command, string destination, string data)
        {
            Message message = new Message();
            message.ReceiverName = destination;
            message.Data = data;
            message.Command = command;
            _action.Invoke(message);
        }

        //internal void RemoveClient(string client)
        //{
        //    _client = _client.Where(x => x.Name != client).ToList();
        //}

        private void OnModelBuilded(Message msg)
        {
            if(IsFiting == false && ClientAgregator.ClientStateIs(Command.ModelBuilded) == true)
            {
                SendTrainData();
            }
        }

        private void OnConnectionReady(Message msg)
        {
            if(_modelBuilder == null) return;

            SendMessageTo(Command.SetAccuracy, msg.SenderName, Serializer.Serialize(_modelBuilder.GetAccuracy()));
            SendMessageTo(Command.SetLoss, msg.SenderName, Serializer.Serialize(_modelBuilder.GetLoss()));
            SendMessageTo(Command.SetFitOptimizer, msg.SenderName, Serializer.Serialize(_modelBuilder.GetOptimizerParam()));
            SendMessageTo(Command.SetLayers, msg.SenderName, Serializer.Serialize(_modelBuilder.GetLayerInfo()));
            SendMessageTo(Command.SetTrainSpeed, msg.SenderName, Serializer.Serialize(_trainSpeed));
            SendMessageTo(Command.BuildModel, msg.SenderName, "");
        }

        private void OnTakeLayerInfo(Message msg)
        {
            _modelsLayerInfo.Add((ILayerInfo[])Serializer.Deserialize(msg.Data));
            if (ClientAgregator.GetClientsNameByState(Command.TakeLayerInfo).Length != _modelsLayerInfo.Count) return;
            ILayerInfo[] combinedInfo = ParallelModelHelper.CombineModels(_modelsLayerInfo.ToArray());
            _modelBuilder.SetLayers(combinedInfo);
            _modelsLayerInfo.Clear();
            SendAnswerMessage(Command.UpdateLayersInfo, Serializer.Serialize(combinedInfo));
        }

        private void OnFitEnded(Message msg)
        {
            _fixClientCounter++;
            int endedCount = ClientAgregator.GetClientsNameByState(Command.FitEnded).Length;
            int notEndedCount = ClientAgregator.GetClientsNameByState(_fitCommand).Length + 
                ClientAgregator.GetClientsNameByState(Command.TakeFitInfo).Length +
                ClientAgregator.GetClientsNameByState(Command.FitInProgress).Length;
            if (notEndedCount > 0)
            {
                return;
            }
            FitInfo fitInfo = (FitInfo)Serializer.Deserialize(msg.Data);
            if(fitInfo.Epoch == _epochToEndFit)
            {
                _fixClientCounter = 0;
                IsFiting = false;
                return;
            }
            _fixClientCounter = 0;
            SendAnswerMessage(Command.GetLayersInfo, "");
        }

        private void OnUpdateInfoEnded(Message msg)
        {
            _fixClientCounter++;
            if (ClientAgregator.GetClientsNameByState(Command.UpdateInfoEnded).Length != _fixClientCounter) return;
            _fixClientCounter = 0;
            Fit();
        }
    }
}
