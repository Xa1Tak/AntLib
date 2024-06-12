using AntLib.Model.Layer;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Model.ModelOptimizer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ILGPU.IR.Analyses.Uniforms;

namespace AntLib.Model
{
    public class ParallelModel : IModel
    {
        private IModel[] _models;
        private IModel _fitedModel;
        private FitInfo[] _modelsFitInfo;
        private bool[] _modelInfoUpdated;
        private ILayerInfo[] _layersInfo;
        private ILayerInfo[][] _layersInfotemp;
        private IFitOptimizer _fitOpt;
        private ILoss _loss;
        private IAccuracy _accuracy;
        private FitInfo _summaryFitInfo;
        private int _updateCount;
        private int _threads;
        private int _totalInfosCount;
        private DateTime _fitStartTime;
        private IModel _m;
        public Action<FitInfo, int> OnUpdate;

        public ParallelModel(int threads)
        {
            _models = new Model[threads];
            _modelsFitInfo = new FitInfo[threads];
            _modelInfoUpdated = new bool[threads];
            _threads = threads;
            _fitedModel = new Model(0);
            _m = this;
        }

        internal void SetFitOptimizer(IFitOptimizer fitOptimizer)
        {
            if (fitOptimizer != null)
            {
                _fitOpt = fitOptimizer;
                _fitedModel.SetFitOptimizer(_fitOpt);
            }
        }

        public void SetUpdateCount(int countInEpoch)
        {
            _updateCount = countInEpoch;
        }

        private void OnModelsUptade(FitInfo info, int index)
        {
            _totalInfosCount++;
            _modelsFitInfo[index] = info;
            _modelInfoUpdated[index] = true;
            FitInfo combinedInfo = ParallelModelHelper.CombineInfo(_modelsFitInfo);
            _summaryFitInfo += combinedInfo;
            var a = _summaryFitInfo.Divide(_totalInfosCount);
            a.Time = (float)(DateTime.Now - _fitStartTime).TotalSeconds;
            if (_modelInfoUpdated.Contains(false) == false)
            {
                OnUpdate?.Invoke(a, index);
                for(int i = 0; i < _modelInfoUpdated.Length; i++)
                {
                    _modelInfoUpdated[i] = false;
                }
            }
        }

        void IModel.SetLayers(ILayerInfo[] layers)
        {
            if (layers != null)
            {
                _fitedModel.SetLayers(layers);
                _layersInfo = _fitedModel.GetLayersInfo();
                for (int i = 0; i < _models.Length; i++)
                {
                    _models[i] = new Model(i);
                    _models[i].SetAccuracy(_accuracy);
                    _models[i].SetLoss(_loss);
                    _models[i].SetFitOptimizer(_fitOpt);
                    _models[i].SetLayers(layers);
                    _models[i].SetOnUpdate((x, num) => OnModelsUptade(x, num));
                }
            }
        }

        void IModel.SetFitOptimizer(IFitOptimizer fitOptimizer)
        {
            if (fitOptimizer != null)
            {
                _fitOpt = fitOptimizer;
                _fitedModel.SetFitOptimizer(_fitOpt);
            }
        }

        void IModel.SetLoss(ILoss loss)
        {
            if (loss != null)
            {
                _loss = loss;
                _fitedModel.SetLoss(_loss);
            }
        }

        void IModel.SetAccuracy(IAccuracy accuracy)
        {
            if (accuracy != null)
            {
                _accuracy = accuracy;
                _fitedModel.SetAccuracy(accuracy);
            }
        }

        void IModel.UpdateLayerInfo(ILayerInfo[] param)
        {
            for(int i = 0; i < _models.Length; i++)
            {
                _models[i].UpdateLayerInfo(param);
            }
            _fitedModel.UpdateLayerInfo(param);
        }

        ILayerInfo[] IModel.GetLayersInfo()
        {
            return _fitedModel.GetLayersInfo();
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, int curEpoch, float trainSpeed)
        {
            _modelsFitInfo = new FitInfo[_threads];
            _layersInfotemp = new ILayerInfo[_models.Length][];
            FitInfo[] modelsFitInfo = new FitInfo[_threads];
            (DataArray[][] XTrainSplited, DataArray[][] YTrainSplited) = ParallelModelHelper.SplitData(XTrain, YTrain, _threads);
            (DataArray[][] XTestSplited, DataArray[][] YTestSplited) = ParallelModelHelper.SplitData(XTest, YTest, _threads);

            _summaryFitInfo = new FitInfo();
            _totalInfosCount = 0;
            Parallel.For(0, _threads, (i) =>
            {
                modelsFitInfo[i] = _models[i].Fit(XTrainSplited[i], YTrainSplited[i], XTestSplited[i], YTestSplited[i], curEpoch, trainSpeed);
                _layersInfotemp[i] = _models[i].GetLayersInfo();
            });

            //_layersInfo = ParallelModelHelper.CombineModels(_layersInfotemp);
            //_fitedModel.UpdateLayerInfo(_layersInfo);

            FitInfo combinedInfo = ParallelModelHelper.CombineInfo(modelsFitInfo);
            combinedInfo.Time = (float)(DateTime.Now - _fitStartTime).TotalSeconds;
            return combinedInfo;
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, int curEpoch, float trainSpeed)
        {
            _modelsFitInfo = new FitInfo[_threads];
            _layersInfotemp = new ILayerInfo[_models.Length][];
            FitInfo[] modelsFitInfo = new FitInfo[_threads];
            (DataArray[][] XTrainSplited, DataArray[][] YTrainSplited) = ParallelModelHelper.SplitData(XTrain, YTrain, _threads);

            _summaryFitInfo = new FitInfo();
            _totalInfosCount = 0;
            Parallel.For(0, _threads, (i) =>
            {
                _models[i].SetStartTime(DateTime.Now);
                modelsFitInfo[i] = _models[i].Fit(XTrainSplited[i], YTrainSplited[i], curEpoch, trainSpeed);
                _layersInfotemp[i] = _models[i].GetLayersInfo();
            });

            //_layersInfo = ParallelModelHelper.CombineModels(_layersInfotemp);
            //_fitedModel.UpdateLayerInfo(_layersInfo);

            FitInfo combinedInfo = ParallelModelHelper.CombineInfo(modelsFitInfo);
            combinedInfo.Time = (float)(DateTime.Now - _fitStartTime).TotalSeconds;
            return combinedInfo;
        }

        void IModel.SetUpdateCount(int countInEpoch)
        {
            _updateCount = countInEpoch;
            foreach (IModel model in _models)
            {
                model.SetUpdateCount(countInEpoch);
            }
        }

        void IModel.SetOnUpdate(Action<FitInfo, int> OnUpdate)
        {
            this.OnUpdate += OnUpdate;
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, float trainSpeed, int epoch)
        {
            FitInfo info = new FitInfo();
            _modelsFitInfo = new FitInfo[_threads];
            _layersInfotemp = new ILayerInfo[_models.Length][];
            _fitStartTime = DateTime.Now;
            for (int e = 1; e <= epoch; e++)
            {
                info = _m.Fit(XTrain, YTrain, XTest, YTest, e, trainSpeed);
                //_m.UpdateLayerInfo(_layersInfo);
            }
            return info;
        }

        FitInfo IModel.Fit(DataArray[] XTrain, DataArray[] YTrain, float trainSpeed, int epoch)
        {
            FitInfo info = new FitInfo();
            _modelsFitInfo = new FitInfo[_threads];
            _layersInfotemp = new ILayerInfo[_models.Length][];
            _fitStartTime = DateTime.Now;
            for (int e = 1; e <= epoch; e++)
            {
                info = _m.Fit(XTrain, YTrain, e, trainSpeed);
                //_m.UpdateLayerInfo(_layersInfo);
            }
            return info;
        }

        DataArray IModel.Predict(DataArray data)
        {
            return _fitedModel.Predict(data);
        }

        FitInfo IModel.Evaluate(DataArray[] X, DataArray[] Y)
        {
            return _fitedModel.Evaluate(X, Y);
        }

        string IModel.ToJson()
        {
            return _fitedModel.ToJson();
        }

        void IModel.SetStartTime(DateTime time)
        {
            _fitStartTime = time;
        }
    }
}
