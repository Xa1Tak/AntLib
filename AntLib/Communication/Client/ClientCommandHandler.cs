using AntLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Model.FitOptimizer;
using AntLib.Tools;
using AntLib.Model.Layer;
using static ILGPU.IR.Analyses.Uniforms;

namespace AntLib.Communication.Client
{
    internal class ClientCommandHandler : CommandHandler
    {
        private Action _buildModel;
        private IModel _model;
        private ModelBuilder _modelBuilder;
        private float _trainSpeed;

        private string _name;

        private DataArray[] _xTrain, _yTrain, _xTest, _yTest;

        internal ClientCommandHandler(string name)
        {
            _name = name;
            _modelBuilder = new ModelBuilder();
            _buildModel += () => { _model = _modelBuilder.BuildModel(); };
        }

        internal void ChooseModel(bool isParallel,int threads)
        {
            _buildModel = null;
            if (isParallel)_buildModel += () => 
            { 
                _model = _modelBuilder.BuildParallelModel(threads);
                //_model.SetOnUpdate((x, y) => SendFitInfo(x));
            };
            else _buildModel += () => 
            { 
                _model = _modelBuilder.BuildModel();
                //_model.SetOnUpdate((x, y) => SendFitInfo(x));
            };

        }

        internal void SendFitInfo(FitInfo info)
        {
            SendAnswerMessage(Command.TakeFitInfo, Serializer.Serialize(info));
        }

        internal void SayReady()
        {
            SendAnswerMessage(Command.ConnectionReady, "");
        }

        internal override void HandleCommand(Message msg)
        {
            if(_name != msg.ReceiverName) return;
            FitInfo info;
            Object obj = Serializer.Deserialize(msg.Data);
            switch (msg.Command)
            {
                case Command.BuildModel:
                    _buildModel();
                    SendAnswerMessage(Command.ModelBuilded, "");
                    break;

                case Command.SetAccuracy:
                    _modelBuilder.SetAccuracy((Accuracy)(Convert.ToInt32(obj)));
                    break;

                case Command.SetLoss:
                    _modelBuilder.SetLoss((Loss)(Convert.ToInt32(obj)));
                    break;

                case Command.SetFitOptimizer:
                    _modelBuilder.SetFitOptimizer((IFitOptimizerParam)obj);
                    break;

                case Command.SetLayers:
                    _modelBuilder.SetLayers((ILayerInfo[])obj);
                    break;

                case Command.SetTrainDataX:
                    _xTrain = (DataArray[])obj;
                    break;

                case Command.SetTrainDataY:
                    _yTrain = (DataArray[])obj;
                    break;

                case Command.SetTestDataX:
                    _xTest = (DataArray[])obj;
                    break;

                case Command.SetTestDataY:
                    _yTest = (DataArray[])obj;
                    break;

                case Command.UpdateLayersInfo:
                    _model.UpdateLayerInfo((ILayerInfo[])obj);
                    SendAnswerMessage(Command.UpdateInfoEnded, "");
                    break;

                case Command.GetLayersInfo:
                    SendAnswerMessage(Command.TakeLayerInfo, _model.ToJson());
                    break;

                case Command.SetOnUpdateCount:
                    _model.SetUpdateCount((int)obj);
                    break;

                case Command.SetTrainSpeed:
                    _trainSpeed = Convert.ToSingle(obj);
                    break;

                case Command.FitWithEval:
                    _model.SetStartTime(DateTime.Now);
                    _model.SetUpdateCount(_xTrain.Length / 10);
                    info = _model.Fit(_xTrain, _yTrain, _xTest, _yTest, Convert.ToInt32(obj) + 1, _trainSpeed);
                    SendFitInfo(info);
                    SendAnswerMessage(Command.FitEnded, Serializer.Serialize(info));
                    break;

                case Command.FitWithoutEval:
                    _model.SetStartTime(DateTime.Now);
                    _model.SetUpdateCount(_xTrain.Length / 10);
                    info = _model.Fit(_xTrain, _yTrain, Convert.ToInt32(obj) + 1, _trainSpeed);
                    SendFitInfo(info);
                    SendAnswerMessage(Command.FitEnded, Serializer.Serialize(info));
                    break;

            }
        }
    }
}
