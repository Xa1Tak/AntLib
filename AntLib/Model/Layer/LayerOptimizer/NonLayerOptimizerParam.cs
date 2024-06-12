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
    public class NonLayerOptimizerParam : ILayerOptimizerParam
    {
        internal float[,] _weights;
        internal float[] _bias;
        internal int _inputCount = 0;
        internal int _outputCount = 0;
        internal float _from = -1;
        internal float _to = 1;

        internal NonLayerOptimizerParam() 
        { 

        }

        public NonLayerOptimizerParam(int inputCount, int outputCount, float from, float to) 
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _from = from;
            _to = to;
            _weights = Matrix.Random(inputCount, outputCount, from, to);
            _bias = Vector.Random(outputCount, from, to);
        }

        public NonLayerOptimizerParam(int inputCount, int outputCount)
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
            _weights = Matrix.Random(inputCount, outputCount, -1f, 1f);
            _bias = Vector.Random(outputCount, -1f, 1f);
        }

        ILayerOptimizer ILayerOptimizerParam.GetOptimizer()
        {
            return new NonLayerOptimizer((NonLayerOptimizerParam)this);
        }

        string ILayerOptimizerParam.GetName()
        {
            return "NonLayerOptimizer";
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
            NonLayerOptimizerParam result = new NonLayerOptimizerParam();
            result._weights = _weights.DeepClone();
            result._bias = _bias.DeepClone();
            result._from = _from;
            result._to = _to;
            return result;
        }

        ILayerOptimizerParam ILayerOptimizerParam.Add(ILayerOptimizerParam param)
        {
            NonLayerOptimizerParam nonParam = param as NonLayerOptimizerParam;
            _weights = _weights.Add(nonParam._weights);
            _bias = _bias.Add(nonParam._bias);
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
