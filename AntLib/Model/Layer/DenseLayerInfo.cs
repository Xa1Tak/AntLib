using AntLib.Model.Layer.ActivationFunction;
using AntLib.Model.Layer.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer
{
    internal class DenseLayerInfo : ILayerInfo
    {
        private IActivationFunc _func;
        private ILayerOptimizer _optimizer;
        
        internal DenseLayerInfo(IActivationFunc func, ILayerOptimizer optimizer)
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

        public ILayer GetLayer()
        {
            return new DenseLayer(_func, _optimizer);
        }

        public string ToJson()
        {
            return "";
        }
    }
}
