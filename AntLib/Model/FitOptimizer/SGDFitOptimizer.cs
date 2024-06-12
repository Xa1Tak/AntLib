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
    internal class SGDFitOptimizer : IFitOptimizer
    {
        private float _chance;

        public SGDFitOptimizer(float chance)
        {
            _chance = chance;
        }

        public string GetName()
        {
            return "SGDFitOptimizer";
        }

        public void OperateLearn(DataArray modelInput, DataArray[] layersOutput, ILayer[] layers, DataArray error, float trainSpeed, int epoch)
        {
            if(StaticRandom.GetNextFloat() < _chance)
            {
                DataArray newError = error;
                for (int i = layers.Length - 1; i > 1; i--)
                {
                    newError = layers[i].Backward(newError, layersOutput[i - 1], layersOutput[i], trainSpeed, epoch);
                }
                layers[0].Backward(error, modelInput, layersOutput[0], trainSpeed, epoch);
            }
        }

        IFitOptimizerParam IFitOptimizer.GetParam()
        {
            return new SGDFitOptimizerParam(_chance);
        }
    }
}
