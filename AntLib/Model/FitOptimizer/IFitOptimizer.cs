using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelOptimizer
{
    public enum FitOptimizer
    {
        None = 0,
        SGD = 1
    }
    public interface IFitOptimizer
    {
        public void OperateLearn(DataArray modelInput, DataArray[] layersOutput, ILayer[] layers, DataArray error, float trainSpeed, int epoch);
        public string GetName();
        public IFitOptimizerParam GetParam();
    }
}
