using AntLib.Model.Layer.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.LayerOptimizer
{
    public interface ILayerOptimizerParam
    {
        internal ILayerOptimizer GetOptimizer();
    }
}
