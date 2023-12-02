using AntLib.Model.ModelAccuracy;
using AntLib.Model.Layer;
using AntLib.Model.ModelLoss;
using AntLib.Model.ModelOptimizer;
using AntLib.Model.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntLib.Model.FitOptimizer;
using static ILGPU.IR.Analyses.Uniforms;

namespace AntLib.Model
{
    public interface IModel
    {
        internal void SetLayers(ILayer[] layers);
        internal void SetFitOptimizer(IFitOptimizer fitOptimizer);
        internal void SetLoss(ILoss loss);
        internal void SetAccuracy(IAccuracy accuracy);
        public void SetUpdateCount(int countInEpoch);
        public FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, float trainSpeed, int epoch);
        public FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, float trainSpeed, int epoch);
        public DataArray Predict(DataArray data);
        public FitInfo Evaluate(DataArray[] X, DataArray[] Y);

    }
}
