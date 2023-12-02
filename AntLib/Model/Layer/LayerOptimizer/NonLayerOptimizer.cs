using Accord.Math;
using AntLib.Model.Layer.Optimizer;
using AntLib.Model.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.LayerOptimizer
{
    internal class NonLayerOptimizer : ILayerOptimizer
    {
        private float[,] _weights;
        private float[] _bias;
        public NonLayerOptimizer(int intputCount, int outputCount, float from, float to) 
        {
            _weights = Matrix.Random(intputCount, outputCount, from, to);
            _bias = Vector.Random(outputCount, from, to);
        }

        public DataArray Backward(DataArray errorData, DataArray inputData, DataArray outputData, float trainSpeed, int epoch)
        {
            throw new NotImplementedException();
        }

        public DataArray Forward(DataArray data)
        {
            throw new NotImplementedException();
        }

        public string GetInputsCount()
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public string GetOutputsCount()
        {
            throw new NotImplementedException();
        }

        public float[] GetParams()
        {
            throw new NotImplementedException();
        }

        public string GetWaightsCount()
        {
            throw new NotImplementedException();
        }
    }
}
