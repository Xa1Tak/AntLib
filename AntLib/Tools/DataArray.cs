using Accord.Math;
using ILGPU.Runtime;
using ILGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILGPU.Runtime.OpenCL;

namespace AntLib.Tools
{
    public class DataArray
    {
        private float[] _array1D;
        private float[,] _array2D;
        private float[,,] _array3D;

        private MemoryBuffer1D<float, Stride1D.Dense> _buffer1D;
        private MemoryBuffer1D<float, Stride1D.Dense> _buffer1DSubtract;

        private MemoryBuffer2D<float, Stride2D.DenseY> _buffer2D;
        private MemoryBuffer3D<float, Stride3D.DenseXY> _buffer3D;

        private string _shape;
        private int _dimensions;

        private Action<Index1D, ArrayView1D<float, Stride1D.Dense>, ArrayView1D<float, Stride1D.Dense>, ArrayView1D<float, Stride1D.Dense>> _subtract1DKernel;

        internal DataArray()
        {

        }

        public DataArray(float[] array1D)
        {
            _array1D = array1D;
            _shape = $"{_array1D.Length}";
            _dimensions = 1;
            if(Booster.IsBoosted() == true)
            {
                _buffer1D = Booster.GetMemoryBuffer1D(_array1D.Length);
                _buffer1D.CopyFromCPU(_array1D);
            }
        }

        public DataArray(float[,] array2D)
        {
            _array2D = array2D;
            _shape = $"{_array2D.GetLength(0)} {_array2D.GetLength(1)}";
            _dimensions = 2;
            if (Booster.IsBoosted() == true)
            {
                _buffer2D = Booster.GetMemoryBuffer2D(_array2D.GetLength(0), _array2D.GetLength(1));
                _buffer2D.CopyFromCPU(_array2D);
            }
        }

        public DataArray(float[,,] array3D)
        {
            _array3D = array3D;
            _shape = $"{_array3D.GetLength(0)} {_array3D.GetLength(1)} {_array3D.GetLength(2)}";
            _dimensions = 3;
            if (Booster.IsBoosted() == true)
            {
                _buffer3D = Booster.GetMemoryBuffer3D(_array3D.GetLength(0), _array3D.GetLength(1), array3D.GetLength(2));
                _buffer3D.CopyFromCPU(_array3D);
            }
        }

        internal DataArray(MemoryBuffer1D<float, Stride1D.Dense> buffer1D)
        {
            _shape = $"{buffer1D.Length}";
            _dimensions = 1;
            _buffer1D = buffer1D;
        }

        internal DataArray(MemoryBuffer2D<float, Stride2D.DenseY> buffer2D)
        {
            _array2D = new float[_buffer2D.IntExtent.X, _buffer2D.IntExtent.Y];
            _buffer2D = buffer2D;
            _shape = $"{_buffer2D.IntExtent.X} {_buffer2D.IntExtent.Y}";
            _dimensions = 2;
        }

        internal DataArray(MemoryBuffer3D<float, Stride3D.DenseXY> buffer3D)
        {
            _buffer3D = buffer3D;
            _shape = $"{buffer3D.IntExtent.X} {buffer3D.IntExtent.Y} {buffer3D.IntExtent.Y}";
            _dimensions = 3;
        }

        public string GetShape()
        {
            return _shape;
        }

        public int GetDimensions()
        {
            return _dimensions;
        }

        public float[] GetArray1D()
        {
            if(Booster.IsBoosted() == true)
            {
                _array1D = _buffer1D.GetAsArray1D();
            }
            return _array1D;
        }

        public float[,] GetArray2D()
        {
            if (Booster.IsBoosted() == true)
            {
                _array2D = new float[_buffer3D.IntExtent.X, _buffer3D.IntExtent.Y];
                _buffer2D.CopyToCPU(_array2D);
            }
            return _array2D;
        }

        public float[,,] GetArray3D()
        {
            if (Booster.IsBoosted() == true)
            {
                _array3D = new float[_buffer3D.IntExtent.X, _buffer3D.IntExtent.Y, _buffer3D.IntExtent.Z];
                _buffer3D.CopyToCPU(_array3D);
            }
            return _array3D;
        }

        internal MemoryBuffer1D<float, Stride1D.Dense> GetBuffer1D()
        {
            return _buffer1D;
        }

        internal MemoryBuffer2D<float, Stride2D.DenseY> GetBuffer2D()
        {
            return _buffer2D;
        }

        internal MemoryBuffer3D<float, Stride3D.DenseXY> GetBuffer3D()
        {
            return _buffer3D;
        }

        public DataArray Subtract(DataArray b)
        {
            if (GetShape() != b.GetShape())
            {
                return null;
            }
            if (_dimensions == 1)
            {
                if(Booster.IsBoosted() == true)
                {
                    Bild1DKernel();
                    return new DataArray(Subtract1D(b));
                }
                return new DataArray(_array1D.Subtract(b._array1D));
            }
            if (_dimensions == 2)
            {
                return new DataArray(_array2D.Subtract(b._array2D));
            }
            return null;
        }

        private MemoryBuffer1D<float, Stride1D.Dense> Subtract1D(DataArray d2)
        {
            _buffer1DSubtract = Booster.GetMemoryBuffer1D((int)_buffer1D.Length);
            _subtract1DKernel(_buffer1D.IntExtent, _buffer1D.View, d2._buffer1D.View, _buffer1DSubtract.View);
            Booster.Sync();
            return _buffer1DSubtract;
        }

        internal void Bild1DKernel()
        {
            if (_subtract1DKernel != null) return;
            _subtract1DKernel = Booster._accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>>(
                Subtract1DKernel);
        }

        private static void Subtract1DKernel(Index1D index, ArrayView1D<float, Stride1D.Dense> input1, ArrayView1D<float, Stride1D.Dense> input2, ArrayView1D<float, Stride1D.Dense> output)
        {
            output[index] = input1[index] - input2[index];
        }
    }
}
