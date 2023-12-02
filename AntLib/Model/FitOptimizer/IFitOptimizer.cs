using AntLib.Model.Layer;
using AntLib.Model.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelOptimizer
{
    enum FitOptimizer
    {
        None = 0,
        SGD = 1
    }
    public interface IFitOptimizer
    {
        public void OperateLearn(DataArray modelInput, DataArray[] layersOutput, ILayer[] layers, DataArray error, float trainSpeed, int epoch);
        public string GetName();
    }
}
