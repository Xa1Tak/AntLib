using Accord.Math.Optimization.Losses;
using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Model.ModelOptimizer;
using AntLib.Model.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ILGPU.IR.Analyses.Uniforms;

namespace AntLib.Model
{
    public class Model
    {
        private ILayer[] _layers;
        private IFitOptimizer _fitOpt;
        private ILoss _loss;
        private IAccuracy _accuracy;
        private int _updateCount;
        public Action<FitInfo> OnUpdate;

        public FitInfo Evaluate(DataArray[] X, DataArray[] Y)
        {
            DataArray prediction;
            DataArray error;
            FitInfo result = new FitInfo(0,0,0,0,0,0,0);
            for(int i = 0; i < X.Length; i++)
            {
                prediction = Predict(X[i]);
                error = prediction - Y[i];
                result.TestLoss += _loss.CalcLoss(error);
                result.TestAccuracy = _accuracy.CalcAccuracy(prediction, Y[i]);
            }
            result.TestLoss /= X.Length;
            return result;
        }

        public DataArray Predict(DataArray data)
        {
            DataArray result;
            result = _layers[0].Forward(data);
            for (int l = 1; l < _layers.Length; l++)
            {
                result = _layers[l].Forward(result);
            }
            return result;
        }

        internal void SetAccuracy(IAccuracy accuracy)
        {
            if(accuracy != null) 
            { 
                _accuracy = accuracy; 
            }
        }

        internal void SetLayers(ILayer[] layers)
        {
            if (layers != null)
            {
                _layers = layers;
            }
        }

        internal void SetLoss(ILoss loss)
        {
            if (loss != null)
            {
                _loss = loss;
            }
        }

        internal void SetFitOptimizer(IFitOptimizer fitOptimizer)
        {
            if (fitOptimizer != null)
            {
                _fitOpt = fitOptimizer;
            }
        }

        public void SetUpdateCount(int countInEpoch)
        {
            _updateCount = countInEpoch;
        }

        public FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, float trainSpeed, int epoch)
        {
            FitInfo info = new FitInfo();
            FitInfo evalInfo = new FitInfo();
            DataArray[] layersOutput = new DataArray[_layers.Length];
            DataArray error;
            DateTime startTime = DateTime.Now;
            for(int e = 1; e <= epoch; e++)
            {
                info.Epoch = e;
                for(int d = 0; d < XTrain.Length; d++)
                {
                    layersOutput[0] = _layers[0].Forward(XTrain[d]);
                    for(int l = 1; l < _layers.Length; l++)
                    {
                        layersOutput[l] = _layers[l].Forward(layersOutput[l - 1]);
                    }
                    error = layersOutput[layersOutput.Length - 1] - YTrain[d];
                    info.TrainLoss += _loss.CalcLoss(error);
                    info.TrainAccuracy = _accuracy.CalcAccuracy(layersOutput[layersOutput.Length - 1], YTrain[d]);
                    _fitOpt.OperateLearn(XTrain[d], layersOutput, _layers, error, trainSpeed, e);
                    if(d % _updateCount == 0 && d != 0)
                    {
                        info.Progress = (float)d / XTrain.Length;
                        info.TrainLoss /= d;
                        info.Time = (float)(DateTime.Now - startTime).TotalSeconds;
                        OnUpdate?.Invoke(info);
                    }
                }
                evalInfo = Evaluate(XTest, YTest);
                info.TestLoss = evalInfo.TestLoss;
                info.TestAccuracy = evalInfo.TestAccuracy;
                OnUpdate?.Invoke(info);
                _accuracy.Reset();
            }
            return info;
        }

        public FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, float trainSpeed, int epoch)
        {
            FitInfo info = new FitInfo();
            DataArray[] layersOutput = new DataArray[_layers.Length];
            DataArray error;
            DateTime startTime = DateTime.Now;
            for (int e = 1; e <= epoch; e++)
            {
                info.Epoch = e;
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
                    _fitOpt.OperateLearn(XTrain[d], layersOutput, _layers, error, trainSpeed, e);
                    if (d % _updateCount == 0 && d != 0)
                    {
                        info.Progress = (float)d / XTrain.Length;
                        info.TrainLoss /= (float)d;
                        info.Time = (float)(DateTime.Now - startTime).TotalSeconds;
                        OnUpdate?.Invoke(info);
                    }
                }
                _accuracy.Reset();
                info.Progress /= (float)XTrain.Length;
                info.TrainLoss /= (float)XTrain.Length;
                info.Time = (float)(DateTime.Now - startTime).TotalSeconds;
                OnUpdate?.Invoke(info);
            }
            return info;
        }
    }
}
