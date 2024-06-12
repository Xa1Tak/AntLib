using Accord.IO;
using Accord.Math;
using AntLib.Model.Layer.Optimizer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.LayerOptimizer
{
    internal class NonLayerOptimizer : ILayerOptimizer
    {
        private NonLayerOptimizerParam _param;
        private float[,] _weights;
        private float[] _bias;
        private float[] _correction;
        private float[] _nextGrad;

        internal NonLayerOptimizer(NonLayerOptimizerParam param) 
        {
            _param = param;
            _weights = param._weights;
            _bias = param._bias;
            _correction = new float[_weights.GetLength(1)];
            _nextGrad = new float[_weights.GetLength(0)];
        }

        public DataArray Backward(DataArray errorData, DataArray inputData, DataArray outputData, float trainSpeed, int epoch)
        {
            float[] e = errorData.GetArray1D();
            float[] input = inputData.GetArray1D();
            _nextGrad.Clear();
            float[] output = outputData.GetArray1D();
            for (int i = 0; i < _correction.Length; i++)
            {
                _correction[i] = e[i] * output[i];
                _bias[i] -= trainSpeed * _correction[i];
                for (int k = 0; k < _weights.GetLength(0); k++)
                {
                    _nextGrad[k] += _weights[k, i] * _correction[i];
                    _weights[k, i] -= trainSpeed * _correction[i];
                }
            }
            return new DataArray(_nextGrad);
        }

        public DataArray Forward(DataArray data)
        {
            return new DataArray(data.GetArray1D().Dot(_weights).Add(_bias));
        }        

        public ILayerOptimizerParam GetParams()
        {
            return _param;
        }

        void ILayerOptimizer.UpdateParams(ILayerOptimizerParam param)
        {
            NonLayerOptimizerParam non = param as NonLayerOptimizerParam;
            _weights = non._weights.DeepClone();
            _bias = non._bias.DeepClone();
        }
    }
}
