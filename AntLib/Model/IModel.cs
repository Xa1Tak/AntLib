using AntLib.Model.ModelAccuracy;
using AntLib.Model.Layer;
using AntLib.Model.ModelLoss;
using AntLib.Model.ModelOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntLib.Model.FitOptimizer;
using static ILGPU.IR.Analyses.Uniforms;
using AntLib.Tools;

namespace AntLib.Model
{
    public interface IModel
    {
        internal void SetLayers(ILayerInfo[] layers);
        internal void SetFitOptimizer(IFitOptimizer fitOptimizer);
        internal void SetLoss(ILoss loss);
        internal void SetAccuracy(IAccuracy accuracy);
        internal void UpdateLayerInfo(ILayerInfo[] param);
        internal void SetStartTime(DateTime time);
        public ILayerInfo[] GetLayersInfo();
        internal FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, int curEpoch, float trainSpeed);
        internal FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, int curEpoch, float trainSpeed);
        public void SetUpdateCount(int countInEpoch);
        public void SetOnUpdate(Action<FitInfo, int> OnUpdate);
        public FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, DataArray[] XTest, DataArray[] YTest, float trainSpeed, int epoch);
        public FitInfo Fit(DataArray[] XTrain, DataArray[] YTrain, float trainSpeed, int epoch);
        public DataArray Predict(DataArray data);
        public FitInfo Evaluate(DataArray[] X, DataArray[] Y);

        public string ToJson();

    }
}
