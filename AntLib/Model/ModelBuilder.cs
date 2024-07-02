using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer;
using AntLib.Model.Layer.ActivationFunction;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.Layer.Optimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Model.ModelOptimizer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model
{
    public class ModelBuilder
    {
        private List<ILayerInfo> _layers;
        private IAccuracy _accuracy;
        private ILoss _loss;
        private IFitOptimizer _fitOpt;

        public ModelBuilder() 
        {
            _layers = new List<ILayerInfo>();
        }

        internal Loss GetLoss()
        {
            return _loss.GetLoss();
        }

        internal Accuracy GetAccuracy()
        {
            return _accuracy.GetAccuracy();
        }

        internal IFitOptimizerParam GetOptimizerParam()
        {
            return _fitOpt.GetParam();
        }

        public IModel BuildModel()
        {
            IModel result = new Model(0);
            result.SetAccuracy( _accuracy );
            result.SetLoss( _loss );
            result.SetFitOptimizer(_fitOpt);
            result.SetLayers(_layers.ToArray());
            return result;
        }

        public IModel BuildModel(string json)
        {
            IModel result = new Model(0);
            result.SetAccuracy(_accuracy);
            result.SetLoss(_loss);
            result.SetFitOptimizer(_fitOpt);
            result.SetLayers((ILayerInfo[])Serializer.Deserialize(json));
            return result;
        }

        public IModel BuildParallelModel(int threads)
        {
            if (Booster.IsBoosted()) return BuildModel();
            IModel result = new ParallelModel(threads);
            result.SetAccuracy(_accuracy);
            result.SetLoss(_loss);
            result.SetFitOptimizer(_fitOpt);
            result.SetLayers(_layers.ToArray());
            return result;
        }

        public IModel BuildParallelModel(int threads, string json)
        {
            IModel result = new ParallelModel(threads);
            result.SetAccuracy(_accuracy);
            result.SetLoss(_loss);
            result.SetFitOptimizer(_fitOpt);
            result.SetLayers((ILayerInfo[])Serializer.Deserialize(json));
            return result;
        }

        public List<ILayerInfo> GetLayers()
        {
            return _layers.Select(l => l.Clone()).ToList();
        }

        public void SetLayers(string json)
        {
            _layers = new List<ILayerInfo>((ILayerInfo[])Serializer.Deserialize(json));
        }

        public void SetLayers(ILayerInfo[] layers)
        {
            _layers = new List<ILayerInfo>(layers);
        }

        public void RemoveLayer(int num)
        {
            if(_layers.Count < num) _layers.RemoveAt(num);
        }

        public void SetAccuracy(Accuracy ac)
        {
            _accuracy = AccuracyProvider.GetAccuracy(ac, null);
        }

        public void SetLoss(Loss loss)
        {
            _loss = LossProvider.GetLoss(loss, null);
        }

        public void SetFitOptimizer(IFitOptimizerParam fitOptimizer)
        {
            _fitOpt = fitOptimizer.GetFitOptimizer();
        }

        public void AddDense(ActivationFunc func, ILayerOptimizerParam opt)
        {
            IActivationFunc activationFunc = ActivationFuncProvider.GetActivationFunc((int)func, null);
            _layers.Add(new DenseLayerInfo(activationFunc, opt));
        }

        public ILayerInfo[] GetLayerInfo()
        {
            return _layers.Select(x => x.Clone()).ToArray();
        }

        public string GetSummaryInfo()
        {
            string result = "";
            foreach (var layer in _layers)
            {
                result += $"{layer.GetInfo()}{Environment.NewLine}";
            }
            result += $"Loss {_loss.GetName()}{Environment.NewLine}";
            result += $"Accuracy {_accuracy.GetName()}{Environment.NewLine}";
            result += $"FitOptimizer {_fitOpt.GetName()}{Environment.NewLine}";
            return result;
        }
    }
}
