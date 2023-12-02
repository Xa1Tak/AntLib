using AntLib.Model.Layer.Optimizer;
using AntLib.Model.ModelOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.FitOptimizer
{
    public interface IFitOptimizerParam
    {
        internal IFitOptimizer GetFitOptimizer();
    }
}
