using AntLib.Model.ModelOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.FitOptimizer
{
    public struct NonFitOptimizerParam : IFitOptimizerParam
    {

        IFitOptimizer IFitOptimizerParam.GetFitOptimizer()
        {
            return new NonFitOptimizer();
        }
    }
}
