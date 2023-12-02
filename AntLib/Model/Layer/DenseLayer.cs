using AntLib.Model.Layer.ActivationFunction;
using AntLib.Model.Layer.Optimizer;
using AntLib.Model.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer
{
    internal class DenseLayer : ILayer
    {
        private IActivationFunc _func;
        private ILayerOptimizer _optimizer;

        internal DenseLayer(IActivationFunc func, ILayerOptimizer optimizer)
        {
            _func = func;
            _optimizer = optimizer;
        }

        public DataArray Forward(DataArray data)
        {
            return _func.Forward(_optimizer.Forward(data));
        }

        public DataArray Backward(DataArray errorData, DataArray inputData, DataArray outputData, float trainSpeed, int epoch)
        {
            return _optimizer.Backward(errorData, inputData, _func.Backward(outputData), trainSpeed, epoch);
        }

        public ILayerInfo GetLayerInfo()
        {
            return new DenseLayerInfo(_func, _optimizer);
        }
    }
}
