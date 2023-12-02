using AntLib.Model.ModelOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.FitOptimizer
{
    public struct SGDFitOptimizerParam : IFitOptimizerParam
    {
        private float _chance = 0.5f;

        public SGDFitOptimizerParam(float chance)
        {
            if (chance > 0 && chance < 1)
            {
                _chance = chance;
            }
        }

        IFitOptimizer IFitOptimizerParam.GetFitOptimizer()
        {
            return new SGDFitOptimizer(_chance);
        }
    }
}
