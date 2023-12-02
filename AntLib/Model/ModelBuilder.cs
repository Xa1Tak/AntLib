using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer;
using AntLib.Model.Layer.ActivationFunction;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.Layer.Optimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Model.ModelOptimizer;
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

        public Model BuildModel()
        {
            Model result = new Model();
            result.SetAccuracy( _accuracy );
            result.SetLoss( _loss );
            result.SetFitOptimizer(_fitOpt);
            result.SetLayers(_layers.Select(x => x.GetLayer()).ToArray());
            return result;
        }

        public IModel BuildParallelModel()
        {
            return null;
        }

        public void RemoveLayer(int num)
        {
            if(_layers.Count < num) _layers.RemoveAt(num);
        }

        public void SetAccuracy(Accuracy ac)
        {
            _accuracy = AccuracyProvider.GetAccuracy((int)ac, null);
        }

        public void SetLoss(Loss loss)
        {
            _loss = LossProvider.GetLoss((int)loss, null);
        }

        public void SetFitOptimizer(IFitOptimizerParam fitOptimizer)
        {
            _fitOpt = fitOptimizer.GetFitOptimizer();
        }

        public void AddDense(ActivationFunc func, ILayerOptimizerParam opt)
        {
            ILayerOptimizer optimizer = opt.GetOptimizer();
            IActivationFunc activationFunc = ActivationFuncProvider.GetActivationFunc((int)func, null);
            _layers.Add(new DenseLayerInfo(activationFunc, optimizer));
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
