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
            _chance = Math.Clamp(chance, 0f, 1f);
        }

        IFitOptimizer IFitOptimizerParam.GetFitOptimizer()
        {
            return new SGDFitOptimizer(_chance);
        }
    }
}
