using Accord.IO;
using Accord.Math;
using AntLib.Model.Layer.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.LayerOptimizer
{
    public class AdamParam : ILayerOptimizerParam
    {
        internal float[,] _weights;
        internal float[] _bias;
        internal float _from = -1;
        internal float _to = 1;
        internal float _e = 0.0000001f;
        internal float _b1 = 0.9f;
        internal float _b2 = 0.999f;

        internal AdamParam()
        {

        }

        public AdamParam(int inputCount, int outputCount, float from, float to, float e, float b1, float b2) 
        {
            _weights = Matrix.Random(inputCount, outputCount, from, to);
            _bias = Vector.Random(outputCount, from, to);
            _from = from;
            _to = to;
            _e = e;
            _b1 = b1;
            _b2 = b2;
        }

        public AdamParam(int inputCount, int outputCount, float from, float to)
        {
            _weights = Matrix.Random(inputCount, outputCount, from, to);
            _bias = Vector.Random(outputCount, from, to);
            _from = from;
            _to = to;
        }

        public AdamParam(int inputCount, int outputCount, float e, float b1, float b2)
        {
            _weights = Matrix.Random(inputCount, outputCount, -1f, 1f);
            _bias = Vector.Random(outputCount, -1f, 1f);
            _e = e;
            _b1 = b1;
            _b2 = b2;
        }

        public AdamParam(int inputCount, int outputCount)
        {
            _weights = Matrix.Random(inputCount, outputCount, -1f, 1f);
            _bias = Vector.Random(outputCount, -1f, 1f);
            _from = -1;
            _to = 1;
        }

        ILayerOptimizer ILayerOptimizerParam.GetOptimizer()
        {
            return new Adam(this);
            //return new Adam((AdamParam)this.Clone());
        }

        string ILayerOptimizerParam.GetName()
        {
            return $"Adam";
        }

        string ILayerOptimizerParam.GetInputsCount()
        {
            return $"{_weights.GetLength(0)}";
        }

        string ILayerOptimizerParam.GetOutputsCount()
        {
            return $"{_weights.GetLength(1)}";
        }

        public ILayerOptimizerParam Clone()
        {
            AdamParam result = new AdamParam();
            result._weights = _weights.DeepClone();
            result._bias = _bias.DeepClone();
            result._b1 = _b1;
            result._b2 = _b2;
            result._e = _e;
            result._from = _from;
            result._to = _to;
            return result;
        }

        ILayerOptimizerParam ILayerOptimizerParam.Add(ILayerOptimizerParam param)
        {
            AdamParam adamParam = param as AdamParam;
            _weights = _weights.Add(adamParam._weights);
            _bias = _bias.Add(adamParam._bias);
            return this;
        }

        ILayerOptimizerParam ILayerOptimizerParam.Divide(int devision)
        {
            _weights = _weights.Divide(devision);
            _bias = _bias.Divide(devision);
            return this;
        }
    }
}
