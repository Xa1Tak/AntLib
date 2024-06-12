using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer
{
    public interface ILayer
    {
        public DataArray Forward(DataArray data);
        public DataArray Backward(DataArray errorData, DataArray inputData, DataArray outputData, float trainSpeed, int epoch);

        public ILayerInfo GetLayerInfo();

        internal void UpdateParams(ILayerInfo param);
    }
}
