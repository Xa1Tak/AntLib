using AntLib.Model.Layer.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.LayerOptimizer
{
    public struct NonLayerOptimizerParam : ILayerOptimizerParam
    {
        private int _inputCount = 0;
        private int _outputCount = 0;
        private float _from = -1;
        private float _to = 1;

        public NonLayerOptimizerParam(int inputCount, int outputCount, float from, float to) 
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _from = from;
            _to = to;
        }

        public NonLayerOptimizerParam(int inputCount, int outputCount)
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
        }

        ILayerOptimizer ILayerOptimizerParam.GetOptimizer()
        {
            return new NonLayerOptimizer(_inputCount, _outputCount, _from, _to);
        }
    }
}
