using AntLib.Model.Layer.ActivationFunction;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.Layer.Optimizer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer
{
    public class DenseLayerInfo : ILayerInfo
    {
        internal IActivationFunc _func;
        internal ILayerOptimizerParam _optimizer;

        internal DenseLayerInfo()
        {

        }
        
        internal DenseLayerInfo(IActivationFunc func, ILayerOptimizerParam optimizer)
        {
            _func = func;
            _optimizer = optimizer;
        }

        public string GetInfo()
        {
            return $"Dense func = {_func.GetName()} opt = {_optimizer.GetName()} inputs = {GetInputsCount()} outputs = {GetOutputsCount()}";
        }

        public string GetWeightsInfo()
        {
            return "";
        }

        public string GetInputsCount()
        {
            return _optimizer.GetInputsCount();
        }

        public string GetOutputsCount()
        {
            return _optimizer.GetOutputsCount();
        }

        public string ToJson()
        {
            return Serializer.Serialize(this);
        }

        ILayerInfo ILayerInfo.Combine(ILayerInfo layerInfo)
        {
            DenseLayerInfo info = layerInfo as DenseLayerInfo;
            ILayerOptimizerParam optimizerParam = info._optimizer.Add(_optimizer).Divide(2);
            return new DenseLayerInfo(_func, optimizerParam);
        }

        ILayerInfo ILayerInfo.Clone()
        {
            return new DenseLayerInfo(_func.Clone(), _optimizer.Clone());
        }

        ILayer ILayerInfo.GetLayer()
        {
            return new DenseLayer(_func, _optimizer.GetOptimizer());
        }
    }
}
