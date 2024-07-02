using AntLib.Model.Layer.Optimizer;
using AntLib.Tools;
using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Backends;
using ILGPU.Runtime;
using ILGPU.Runtime.OpenCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.LayerOptimizer
{
    internal class AdamGPU : ILayerOptimizer
    {
        private AdamParam _param;

        private float _e;
        private float _b1;
        private float _bb1;
        private float _b2;
        private float _bb2;

        private MemoryBuffer2D<float, Stride2D.DenseY> _weights;
        private MemoryBuffer2D<float, Stride2D.DenseY> _mtWeights;
        private MemoryBuffer2D<float, Stride2D.DenseY> _vtWeights;

        private MemoryBuffer1D<float, Stride1D.Dense> _bias;
        private MemoryBuffer1D<float, Stride1D.Dense> _vtBias;
        private MemoryBuffer1D<float, Stride1D.Dense> _mtBias;

        private MemoryBuffer1D<float, Stride1D.Dense> _forwardOutput;
        private MemoryBuffer1D<float, Stride1D.Dense> _backwardOutput;

        private MemoryBuffer1D<float, Stride1D.Dense> _internalParams;

        private Action<
            Index1D, 
            ArrayView2D<float, Stride2D.DenseY>,
            ArrayView1D<float, Stride1D.Dense>, 
            ArrayView1D<float, Stride1D.Dense>,
            ArrayView1D<float, Stride1D.Dense>> _forwardKernel;

        private Action<
                Index1D,
                ArrayView2D<float, Stride2D.DenseY>,
                ArrayView2D<float, Stride2D.DenseY>,
                ArrayView2D<float, Stride2D.DenseY>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                float, int> _backwardAdamKernel;

        internal AdamGPU(AdamParam param)
        {
            _param = param;
            _e = param._e;
            _b1 = param._b1;
            _b2 = param._b2;
            Build();
        }

        private void Build()
        {
            _weights = Booster.GetMemoryBuffer2D(_param._weights.GetLength(0), _param._weights.GetLength(1));
            _weights.CopyFromCPU(_param._weights);
            _mtWeights = Booster.GetMemoryBuffer2D(_param._weights.GetLength(0), _param._weights.GetLength(1));
            _vtWeights = Booster.GetMemoryBuffer2D(_param._weights.GetLength(0), _param._weights.GetLength(1));

            _bias = Booster.GetMemoryBuffer1D(_param._bias.Length);
            _bias.CopyFromCPU(_param._bias);
            _vtBias = Booster.GetMemoryBuffer1D(_param._bias.Length);
            _mtBias = Booster.GetMemoryBuffer1D(_param._bias.Length);

            _forwardOutput = Booster.GetMemoryBuffer1D(_param._weights.GetLength(1));
            _backwardOutput = Booster.GetMemoryBuffer1D(_param._weights.GetLength(0));

            _internalParams = Booster.GetMemoryBuffer1D(3);
            float[] tempParams = { _e, _b1, _b2 };
            _internalParams.CopyFromCPU(tempParams);

            if (_forwardKernel == null)
            {
                _forwardKernel = Booster._accelerator.LoadAutoGroupedStreamKernel<
                    Index1D,
                    ArrayView2D<float, Stride2D.DenseY>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>>(
                    ForwardKernel1);
            }
            if (_backwardAdamKernel == null)
            {
                _backwardAdamKernel = Booster._accelerator.LoadAutoGroupedStreamKernel<
                    Index1D,
                    ArrayView2D<float, Stride2D.DenseY>,
                    ArrayView2D<float, Stride2D.DenseY>,
                    ArrayView2D<float, Stride2D.DenseY>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    ArrayView1D<float, Stride1D.Dense>,
                    float, int>(
                    BackwardAdamKernel1);
            }
        }

        DataArray ILayerOptimizer.Backward(DataArray errorData, DataArray inputData, DataArray outputData, float trainSpeed, int epoch)
        {
            _backwardOutput.MemSetToZero();
            _backwardAdamKernel(_forwardOutput.IntExtent,
                _weights.View,
                _mtWeights.View,
                _vtWeights.View,
                _bias.View,
                _mtBias.View,
                _vtBias.View,
                errorData.GetBuffer1D().View,
                inputData.GetBuffer1D().View,
                outputData.GetBuffer1D().View,
                _backwardOutput.View,
                _internalParams.View,
                trainSpeed, epoch);
            Booster.Sync();
            return new DataArray(_backwardOutput);
        }

        DataArray ILayerOptimizer.Forward(DataArray data)
        {
            //Console.WriteLine($"input {data.GetArray1D()[0]} {data.GetArray1D()[1]} {data.GetArray1D()[2]}");
            //_forwardOutput.CopyFrom(_bias);
            //Console.WriteLine($"just bias {_forwardOutput.View.GetAsArray1D()[0]} {_forwardOutput.View.GetAsArray1D()[1]} {_forwardOutput.View.GetAsArray1D()[2]}");
            _forwardKernel(_forwardOutput.IntExtent, _weights.View, _bias.View, data.GetBuffer1D().View, _forwardOutput.View);
            Booster.Sync();
            //Console.WriteLine($"output {_forwardOutput.View.GetAsArray1D()[0]} {_forwardOutput.View.GetAsArray1D()[1]} {_forwardOutput.View.GetAsArray1D()[2]}");
            //Console.WriteLine("---------------");
            return new DataArray(_forwardOutput);
        }

        ILayerOptimizerParam ILayerOptimizer.GetParams()
        {
            _weights.CopyToCPU(_param._weights);
            _bias.CopyToCPU(_param._bias);
            return _param;
        }

        void ILayerOptimizer.UpdateParams(ILayerOptimizerParam param)
        {
            AdamParam adam = param as AdamParam;
            _weights.CopyFromCPU(adam._weights);
            _bias.CopyFromCPU(adam._bias);
        }

        private static void ForwardKernel(Index1D index, ArrayView2D<float, Stride2D.DenseY> matrixView, ArrayView1D<float, Stride1D.Dense> biasView, ArrayView1D<float, Stride1D.Dense> inputView, ArrayView1D<float, Stride1D.Dense> resultView)
        {
            int x = index.X % inputView.IntLength;
            int y = index.X / inputView.IntLength;
            resultView[y] += (matrixView[x, y] * inputView[x]);

        }

        private static void ForwardKernel1(Index1D index, ArrayView2D<float, Stride2D.DenseY> matrixView, ArrayView1D<float, Stride1D.Dense> biasView, ArrayView1D<float, Stride1D.Dense> inputView, ArrayView1D<float, Stride1D.Dense> resultView)
        {
            resultView[index] = biasView[index];
            for (int i = 0; i < inputView.Length; i++)
                resultView[index] += matrixView[i, index] * inputView[i];
        }

        private static void BackwardAdamKernel(Index1D index,
        ArrayView2D<float, Stride2D.DenseY> weightsView,
        ArrayView2D<float, Stride2D.DenseY> mtweightsView,
        ArrayView2D<float, Stride2D.DenseY> vtweightsView,
        ArrayView1D<float, Stride1D.Dense> biasView,
        ArrayView1D<float, Stride1D.Dense> mtbiasView,
        ArrayView1D<float, Stride1D.Dense> vtbiasView,
        ArrayView1D<float, Stride1D.Dense> errorView,
        ArrayView1D<float, Stride1D.Dense> inputView,
        ArrayView1D<float, Stride1D.Dense> outputView,
        ArrayView1D<float, Stride1D.Dense> nextGradView,
        ArrayView1D<float, Stride1D.Dense> paramsView,
        float _trainSpeed,int epoch)
        {
            int x = index.X % inputView.IntLength;
            int y = index.X / inputView.IntLength;

            float _bb2 = 1 - paramsView[2];
            float _bb1 = 1 - paramsView[1];
            float bb1Pow = 1 - XMath.Pow(paramsView[1], epoch);
            float bb2Pow = 1 - XMath.Pow(paramsView[2], epoch);

            float _correction = errorView[y] * outputView[y];
            float _correction2 = _correction * _correction;
            vtbiasView[y] = (paramsView[2] * vtbiasView[y]) + (_bb2 * _correction2);
            mtbiasView[y] = (paramsView[1] * mtbiasView[y]) + (_bb1 * _correction);
            biasView[y] -= _trainSpeed * ((mtbiasView[y] / bb1Pow) / (XMath.Sqrt((vtbiasView[y] / bb2Pow) + paramsView[0])));

            nextGradView[x] += weightsView[x, y] * _correction;
            vtweightsView[x, y] = (paramsView[2] * vtweightsView[x, y]) + (_bb2 * _correction2 * inputView[x] * inputView[x]);
            mtweightsView[x, y] = (paramsView[1] * mtweightsView[x, y]) + (_bb1 * _correction * inputView[x]);
            weightsView[x, y] -= _trainSpeed * ((mtweightsView[x, y] / bb1Pow) / (XMath.Sqrt((vtweightsView[x, y] / bb2Pow) + paramsView[0])));
        }

        private static void BackwardAdamKernel1(Index1D index,
        ArrayView2D<float, Stride2D.DenseY> weightsView,
        ArrayView2D<float, Stride2D.DenseY> mtweightsView,
        ArrayView2D<float, Stride2D.DenseY> vtweightsView,
        ArrayView1D<float, Stride1D.Dense> biasView,
        ArrayView1D<float, Stride1D.Dense> mtbiasView,
        ArrayView1D<float, Stride1D.Dense> vtbiasView,
        ArrayView1D<float, Stride1D.Dense> errorView,
        ArrayView1D<float, Stride1D.Dense> inputView,
        ArrayView1D<float, Stride1D.Dense> outputView,
        ArrayView1D<float, Stride1D.Dense> nextGradView,
        ArrayView1D<float, Stride1D.Dense> paramsView,
        float _trainSpeed, int epoch)
        {
            float _bb2 = 1 - paramsView[2];
            float _bb1 = 1 - paramsView[1];
            float bb1Pow = 1 - XMath.Pow(paramsView[1], epoch);
            float bb2Pow = 1 - XMath.Pow(paramsView[2], epoch);

            float _correction = errorView[index] * outputView[index];
            float _correction2 = _correction * _correction;
            vtbiasView[index] = paramsView[2] * vtbiasView[index] + (_bb2 * _correction2);
            mtbiasView[index] = paramsView[1] * mtbiasView[index] + (_bb1 * _correction);
            biasView[index] -= _trainSpeed * ((mtbiasView[index] / bb1Pow) / (XMath.Sqrt((vtbiasView[index] / bb2Pow) + paramsView[0])));

            for (int k = 0; k < inputView.Length; k++)
            {
                nextGradView[k] += weightsView[k, index] * _correction;
                vtweightsView[k, index] = paramsView[2] * vtweightsView[k, index] + (_bb2 * _correction2 * inputView[k] * inputView[k]);
                mtweightsView[k, index] = paramsView[1] * mtweightsView[k, index] + (_bb1 * _correction * inputView[k]);
                weightsView[k, index] -= _trainSpeed * ((mtweightsView[k, index] / bb1Pow) / (XMath.Sqrt((vtweightsView[k, index] / bb2Pow) + paramsView[0])));
            }
        }

    }
}
