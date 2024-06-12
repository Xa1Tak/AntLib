using AntLib.Model.Layer.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer
{
    public interface ILayerInfo
    {
        public string GetInfo();
        public string GetWeightsInfo();

        public string GetInputsCount();

        public string GetOutputsCount();

        internal ILayer GetLayer();

        internal ILayerInfo Combine(ILayerInfo layerInfo);

        public string ToJson();

        public ILayerInfo Clone();

    }
}
