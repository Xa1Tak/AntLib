using AntLib.Model.Layer.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.LayerOptimizer
{
    public struct AdamParam : ILayerOptimizerParam
    {
        private int _inputCount = 0;
        private int _outputCount = 0;
        private float _from = -1;
        private float _to = 1;
        private float _e = 0.0000001f;
        private float _b1 = 0.9f;
        private float _b2 = 0.999f;
        public AdamParam(int inputCount, int outputCount, float from, float to, float e, float b1, float b2) 
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _from = from;
            _to = to;
            _e = e;
            _b1 = b1;
            _b2 = b2;
        }

        public AdamParam(int inputCount, int outputCount, float from, float to)
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _from = from;
            _to = to;
        }

        public AdamParam(int inputCount, int outputCount, float e, float b1, float b2)
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _e = e;
            _b1 = b1;
            _b2 = b2;
        }

        public AdamParam(int inputCount, int outputCount)
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _from = -1;
            _to = 1;
        }

        ILayerOptimizer ILayerOptimizerParam.GetOptimizer()
        {
            return new Adam(_inputCount, _outputCount, _from, _to, _e, _b1, _b2);
        }
    }
}
