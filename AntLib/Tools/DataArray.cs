using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Tools
{
    public class DataArray
    {
        private float[] _array1D;
        private float[,] _array2D;
        private float[,,] _array3D;
        private string _shape;
        private int _dimensions;

        internal DataArray()
        {

        }

        public DataArray(float[] array1D)
        {
            _array1D = array1D;
            _shape = $"{_array1D.Length}";
            _dimensions = 1;
        }

        public DataArray(float[,] array2D)
        {
            _array2D = array2D;
            _shape = $"{_array2D.GetLength(0)} {_array2D.GetLength(1)}";
            _dimensions = 2;
        }

        public DataArray(float[,,] array3D)
        {
            _array3D = array3D;
            _shape = $"{_array3D.GetLength(0)} {_array3D.GetLength(1)} {_array3D.GetLength(2)}";
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
            return _array1D;
        }

        public float[,] GetArray2D()
        {
            return _array2D;
        }

        public float[,,] GetArray3D()
        {
            return _array3D;
        }

        public static DataArray operator -(DataArray a, DataArray b)
        {
            if (a.GetShape() != b.GetShape())
            {
                return null;
            }
            if (a._dimensions == 1)
            {
                return new DataArray(a._array1D.Subtract(b._array1D));
            }
            if (a._dimensions == 2)
            {
                return new DataArray(a._array2D.Subtract(b._array2D));
            }
            return null;
        }
    }
}
