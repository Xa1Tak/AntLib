using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.ModelOptimizer;
using AntLib.Tools;
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
        public ILayerOptimizerParam GetParams();
        public void UpdateParams(ILayerOptimizerParam param);
    }
}
