using AntLib.Model.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.Optimizer
{
    internal interface ILayerOptimizer
    {
        public DataArray Forward(DataArray data);
        public DataArray Backward(DataArray errorData, DataArray inputData, DataArray outputData, float trainSpeed, int epoch);
        public string GetInputsCount();
        public string GetOutputsCount();
        public string GetWaightsCount();
        public string GetName();
        public float[] GetParams();
    }
}
