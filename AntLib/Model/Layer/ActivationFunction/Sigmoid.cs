using Accord.Math;
using AntLib.Tools;
using ILGPU.Runtime;
using ILGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU.Runtime.OpenCL;
using ILGPU.Algorithms;

namespace AntLib.Model.Layer.ActivationFunction
{
    internal class Sigmoid : IActivationFunc
    {
        private Action<Index1D, ArrayView1D<float, Stride1D.Dense>, ArrayView1D<float, Stride1D.Dense>> _sigmoid1DForwardKernel;
        private static Action<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>> _sigmoid2DForwardKernel;
        private static Action<Index3D, ArrayView3D<float, Stride3D.DenseXY>, ArrayView3D<float, Stride3D.DenseXY>> _sigmoid3DForwardKernel;

        private Action<Index1D, ArrayView1D<float, Stride1D.Dense>, ArrayView1D<float, Stride1D.Dense>> _sigmoid1DBackwardKernel;
        private static Action<Index2D, ArrayView2D<float, Stride2D.DenseX>, ArrayView2D<float, Stride2D.DenseX>> _sigmoid2DBackwardKernel;
        private static Action<Index3D, ArrayView3D<float, Stride3D.DenseXY>, ArrayView3D<float, Stride3D.DenseXY>> _sigmoid3DBackwardKernel;

        private MemoryBuffer1D<float, Stride1D.Dense> _output1D;

        public DataArray Forward(DataArray data)
        {
            if(data.GetDimensions() == 1)
            {
                if(Booster.IsBoosted() == true)
                {
                    Build1D(data);
                    _sigmoid1DForwardKernel(data.GetBuffer1D().IntExtent, data.GetBuffer1D().View, _output1D.View);
                    Booster.Sync();
                    return new DataArray(_output1D);

                }
                return new DataArray(data.GetArray1D().Apply(x => (float)(1 / (1 + Math.Pow(Math.E, -1 * x)))));
                //return new DataArray(data.GetArray1D().Apply(x => 1 / (1 + XMath.Exp(-x))));
            }

            if(data.GetDimensions() == 2)
            {
                return new DataArray(data.GetArray2D().Apply(x => (float)(1 / (1 + Math.Pow(Math.E, -x)))));
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
                    _sigmoid1DBackwardKernel(data.GetBuffer1D().IntExtent, data.GetBuffer1D().View, _output1D.View);
                    Booster.Sync();
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
            return "Sigmoid";
        }

        IActivationFunc IActivationFunc.Clone()
        {
            return new Sigmoid();
        }

        private void Build1D(DataArray data)
        {
            if(_output1D == null)
            {
                int outputLength = Convert.ToInt32(data.GetShape());
                _output1D = Booster.GetMemoryBuffer1D(outputLength);
            }
            if(_sigmoid1DForwardKernel != null) return;
            _sigmoid1DForwardKernel = Booster._accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>>(
                Sigmoid1DForwardKernel);

            _sigmoid1DBackwardKernel = Booster._accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>>(
                Sigmoid1DBackwardKernel);
        }

        private static void Sigmoid1DForwardKernel(Index1D index, ArrayView1D<float, Stride1D.Dense> input, ArrayView1D<float, Stride1D.Dense> output)
        {
            output[index] = 1 / (1 + XMath.Exp(-1 * input[index])); //(float)Math.Pow(Math.E, -1 * input[index]));
        }

        private static void Sigmoid1DBackwardKernel(Index1D index, ArrayView1D<float, Stride1D.Dense> input, ArrayView1D<float, Stride1D.Dense> output)
        {
            output[index] = input[index] * (1 - input[index]);
        }
    }
}
