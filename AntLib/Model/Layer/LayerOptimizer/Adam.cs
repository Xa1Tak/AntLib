using Accord.IO;
using Accord.Math;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Tools;
using ILGPU.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.Optimizer
{
    internal class Adam : ILayerOptimizer
    {
        private AdamParam _param;

        private float[,] _weights;
        private float[,] _vtWeights;
        private float[,] _mtWeights;
        
        private float[] _bias;
        private float[] _vtBias;
        private float[] _mtBias;
        
        private float _e;
        private float _b1;
        private float _bb1;
        private float _b2;
        private float _bb2;

        private float[] _correction;
        private float[] _correction2;
        private float[] _nextGrad;

        internal Adam(AdamParam param)
        {
            _param = param;
            _e = param._e;
            _b1 = param._b1;
            _b2 = param._b2;
            _weights = param._weights;
            _bias = param._bias;
            Build(_weights.GetLength(0), _weights.GetLength(1));
        }

        private void Build(int x, int y)
        {
            _bb2 = 1 - _b2;
            _bb1 = 1 - _b1;
            _vtWeights = new float[x,y];
            _mtWeights = new float[x,y];
            _vtBias = new float[y];
            _mtBias = new float[y];
            _correction = new float[y];
            _correction2 = new float[y];
            _nextGrad = new float[x];
        }

        public DataArray Forward(DataArray data)
        {
            return new DataArray(data.GetArray1D().Dot(_weights).Add(_bias));
        }

        public DataArray Backward(DataArray errorData, DataArray inputData, DataArray outputData, float trainSpeed, int epoch)
        {
            float bb1Pow = 1 - XMath.Pow(_b1, epoch);
            float bb2Pow = 1 - XMath.Pow(_b2, epoch);
            _nextGrad.Clear();
            float[] e = errorData.GetArray1D();
            float[] input = inputData.GetArray1D();
            float[] output = outputData.GetArray1D();
            for (int i = 0; i < _correction.Length; i++)
            {
                _correction[i] = e[i] * output[i];
                _correction2[i] = _correction[i] * _correction[i];
                _vtBias[i] = _b2 * _vtBias[i] + (_bb2 * _correction2[i]);
                _mtBias[i] = _b1 * _mtBias[i] + (_bb1 * _correction[i]);
                _bias[i] -= trainSpeed * ((_mtBias[i] / bb1Pow) / (XMath.Sqrt((_vtBias[i] / bb2Pow) + _e)));

                for (int k = 0; k < _weights.GetLength(0); k++)
                {
                    _nextGrad[k] += _weights[k, i] * _correction[i];
                    _vtWeights[k, i] = _b2 * _vtWeights[k, i] + (_bb2 * _correction2[i] * input[k] * input[k]);
                    _mtWeights[k, i] = _b1 * _mtWeights[k, i] + (_bb1 * _correction[i] * input[k]);
                    _weights[k, i] -= trainSpeed * ((_mtWeights[k, i] / bb1Pow) / (XMath.Sqrt((_vtWeights[k, i] / bb2Pow) + _e)));
                }
            }
            return new DataArray(_nextGrad);
        }

        public ILayerOptimizerParam GetParams()
        {
            return _param;
        }

        void ILayerOptimizer.UpdateParams(ILayerOptimizerParam param)
        {
            AdamParam adam = param as AdamParam;
            _weights = adam._weights;
            _bias = adam._bias;
            //Build(_weights.GetLength(0), _weights.GetLength(1));
        }
    }
}
