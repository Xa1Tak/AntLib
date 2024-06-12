using AntLib.Model.Layer;
using AntLib.Model.ModelOptimizer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.FitOptimizer
{
    internal class NonFitOptimizer : IFitOptimizer
    {
        public string GetName()
        {
            return "NonFitOptimizer";
        }

        public void OperateLearn(DataArray modelInput, DataArray[] layersOutput, ILayer[] layers, DataArray error, float trainSpeed, int epoch)
        {
            DataArray newError = error;
            for(int i = layers.Length - 1; i > 0; i--)
            {
                newError = layers[i].Backward(newError, layersOutput[i - 1], layersOutput[i], trainSpeed, epoch);
            }
            layers[0].Backward(newError, modelInput, layersOutput[0], trainSpeed, epoch);
        }

        IFitOptimizerParam IFitOptimizer.GetParam()
        {
            return new NonFitOptimizerParam();
        }
    }
}
