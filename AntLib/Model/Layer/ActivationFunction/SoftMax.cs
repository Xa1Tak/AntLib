using Accord.Math;
using AntLib.Tools;
using ILGPU.Runtime;
using ILGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU.Algorithms;

namespace AntLib.Model.Layer.ActivationFunction
{
    internal class SoftMax : IActivationFunc
    {
        private static Action<Index1D, ArrayView1D<float, Stride1D.Dense>, ArrayView1D<float, Stride1D.Dense>> _softmax1DForwardKernel;
        private static Action<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>> _softmax2DForwardKernel;
        private static Action<Index3D, ArrayView3D<float, Stride3D.DenseXY>, ArrayView3D<float, Stride3D.DenseXY>> _softmax3DForwardKernel;

        private static Action<Index1D, ArrayView1D<float, Stride1D.Dense>, ArrayView1D<float, Stride1D.Dense>> _softmax1DBackwardKernel;
        private static Action<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>> _softmax2DBackwardKernel;
        private static Action<Index3D, ArrayView3D<float, Stride3D.DenseXY>, ArrayView3D<float, Stride3D.DenseXY>> _softmax3DBackwardKernel;

        private MemoryBuffer1D<float, Stride1D.Dense> _output1D;

        public DataArray Forward(DataArray data)
        {
            if(data.GetDimensions() == 1)
            {
                if (Booster.IsBoosted() == true)
                {
                    Build1D(data);
                    _softmax1DForwardKernel(data.GetBuffer1D().IntExtent, data.GetBuffer1D().View, _output1D.View);
                    //Booster._accelerator.Synchronize();
                    return new DataArray(_output1D);
                }
                return new DataArray(Special.Softmax(data.GetArray1D().Apply(x => (double)x)).Apply(x => (float)x));
                //return new DataArray(data.GetArray1D().Apply(x => (float)(1 / (1 + Math.Pow(Math.E, -x)))));
            }

            return data;

        }
        public DataArray Backward(DataArray data)
        {
            if(data.GetDimensions() == 1)
            {
                if (Booster.IsBoosted() == true)
                {
                    Build1D(data);
                    _softmax1DBackwardKernel(data.GetBuffer1D().IntExtent, data.GetBuffer1D().View, _output1D.View);
                    Booster._accelerator.Synchronize();
                    return new DataArray(_output1D);
                }
                return new DataArray(data.GetArray1D().Apply(x => x * (1 - x)));
            }

            if(data.GetDimensions() == 2)
            {
                return new DataArray(data.GetArray2D().Apply(x => x * (1 - x)));
            }

            return data;
        }
        public string GetName()
        {
            return "SoftMax";
        }

        IActivationFunc IActivationFunc.Clone()
        {
            return new SoftMax();
        }

        private void Build1D(DataArray data)
        {
            if (_output1D == null)
            {
                int outputLength = Convert.ToInt32(data.GetShape());
                _output1D = Booster.GetMemoryBuffer1D(outputLength);
            }
            if (_softmax1DForwardKernel != null) return;
            _softmax1DForwardKernel = Booster._accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>>(
                Softmax1DForwardKernel);

            _softmax1DBackwardKernel = Booster._accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>>(
                Softmax1DBackwardKernel);
        }

        private static void Softmax1DForwardKernel(Index1D index, ArrayView1D<float, Stride1D.Dense> input, ArrayView1D<float, Stride1D.Dense> output)
        {
            float sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                //sum += (float)Math.Pow(Math.E, input[i]);
                sum += XMath.Exp(input[i]);
            }
            //output[index] = (float)Math.Pow(Math.E, input[index]) / sum;
            output[index] = XMath.Exp(input[index]) / sum;
        }

        private static void Softmax1DBackwardKernel(Index1D index, ArrayView1D<float, Stride1D.Dense> input, ArrayView1D<float, Stride1D.Dense> output)
        {
            output[index] = input[index] * (1 - input[index]);
        }
    }
}
