using Accord.Math.Optimization.Losses;
using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Model.ModelOptimizer;
using AntLib.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ILGPU.IR.Analyses.Uniforms;

namespace AntLib.Model
{
    public class Model : IModel
    {
        private ILayer[] _layers;
        private IFitOptimizer _fitOpt;
        private ILoss _loss;
        private IAccuracy _accuracy;
        private int _updateCount;
        private int _modelNum;
        private DateTime _fitStartTime;
        private Action<FitInfo, int> OnUpdate;

        private IModel _m;

        public Model(int num)
        {
            _modelNum = num;
            _m = this;
        }

        void IModel.SetLayers(ILayerInfo[] layers)
        {
            if (layers != null)
            {
                _layers = new ILayer[layers.Length];
                for (int i = 0; i < layers.Length; i++)
                {
                    _layers[i] = layers[i].GetLayer();
                }
            }
        }

        void IModel.SetFitOptimizer(IFitOptimizer fitOptimizer)
        {
            if (fitOptimizer != null)
            {
                _fitOpt = fitOptimizer;
            }
        }

        void IModel.SetLoss(ILoss loss)
        {
            if (loss != null)
            {
                _loss = loss;
            }
        }

        void IModel.SetAccuracy(IAccuracy accuracy)
        {
            if (accuracy != null)
            {
                _accuracy = accuracy;
            }
        }

        ILayerInfo[] IModel.GetLayersInfo()
        {
            ILayerInfo[] result = new ILayerInfo[_layers.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = _layers[i].GetLayerInfo().Clone();
            }
            return result;
        }

        void IModel.SetUpdateCount(int countInEpoch)
        {
            _updateCount = countInEpoch;
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, float trainSpeed, int epoch)
        {
            FitInfo info = new FitInfo();
            _fitStartTime = DateTime.Now;
            for (int e = 1; e <= epoch; e++)
            {
                info = _m.Fit(XTrain, YTrain, XTest, YTest, e, trainSpeed);
            }
            return info;
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, float trainSpeed, int epoch)
        {
            FitInfo info = new FitInfo();
            _fitStartTime = DateTime.Now;
            for (int e = 1; e <= epoch; e++)
            {
                info = _m.Fit(XTrain, YTrain, e, trainSpeed);
            }
            return info;
        }

        DataArray IModel.Predict(DataArray data)
        {
            DataArray result;
            result = _layers[0].Forward(data);
            for (int l = 1; l < _layers.Length; l++)
            {
                result = _layers[l].Forward(result);
            }
            return result;
        }

        FitInfo IModel.Evaluate(DataArray[] X, DataArray[] Y)
        {
            DataArray prediction;
            DataArray error;
            FitInfo result = new FitInfo(1, 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < X.Length; i++)
            {
                prediction = _m.Predict(X[i]);
                error = prediction - Y[i];
                result.TestLoss += _loss.CalcLoss(error);
                result.TestAccuracy = _accuracy.CalcAccuracy(prediction, Y[i]);
            }
            result.TestLoss /= X.Length;
            return result;
        }

        void IModel.UpdateLayerInfo(ILayerInfo[] param)
        {
            for (int i = 0; i < param.Length; i++)
            {
                _layers[i].UpdateParams(param[i]);
            }
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, int curEpoch, float trainSpeed)
        {
            FitInfo evalInfo = new FitInfo();

            FitInfo info = _m.Fit(XTrain, YTrain, curEpoch, trainSpeed);
            evalInfo = _m.Evaluate(XTest, YTest);
            info.Time = (float)(DateTime.Now - _fitStartTime).TotalSeconds;
            info.TestLoss = evalInfo.TestLoss;
            info.TestAccuracy = evalInfo.TestAccuracy;
            OnUpdate?.Invoke(info, _modelNum);
            _accuracy.Reset();
            return info;
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, int curEpoch, float trainSpeed)
        {
            FitInfo info = new FitInfo();
            DataArray[] layersOutput = new DataArray[_layers.Length];
            DataArray error;
            info.Epoch = curEpoch;
            for (int d = 0; d < XTrain.Length; d++)
            {
                layersOutput[0] = _layers[0].Forward(XTrain[d]);
                for (int l = 1; l < _layers.Length; l++)
                {
                    layersOutput[l] = _layers[l].Forward(layersOutput[l - 1]);
                }
                error = layersOutput[layersOutput.Length - 1] - YTrain[d];
                info.TrainLoss += _loss.CalcLoss(error);
                info.TrainAccuracy = _accuracy.CalcAccuracy(layersOutput[layersOutput.Length - 1], YTrain[d]);
                _fitOpt.OperateLearn(XTrain[d], layersOutput, _layers, error, trainSpeed, curEpoch);
                if (_updateCount != 0 && d != 0 && d % _updateCount == 0)
                {
                    info.Progress = (float)d / XTrain.Length;
                    info.TrainLoss /= d;
                    info.Time = (float)(DateTime.Now - _fitStartTime).TotalSeconds;
                    OnUpdate?.Invoke(info, _modelNum);
                }
            }
            _accuracy.Reset();
            info.Progress /= XTrain.Length;
            info.TrainLoss /= XTrain.Length;
            info.Time = (float)(DateTime.Now - _fitStartTime).TotalSeconds;
            OnUpdate?.Invoke(info, _modelNum);
            return info;
        }

        public void SetOnUpdate(Action<FitInfo, int> OnUpdate)
        {
            this.OnUpdate += OnUpdate;
        }

        string IModel.ToJson()
        {
            return Serializer.Serialize(_layers.Select(x => x.GetLayerInfo()).ToArray());
        }

        void IModel.SetStartTime(DateTime time)
        {
            _fitStartTime = time;
        }
    }
}
