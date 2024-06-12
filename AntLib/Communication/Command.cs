using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Communication
{
    public enum Command
    {
        ConnectionReady,

        SetTrainDataX,
        SetTrainDataY,
        SetTestDataX,
        SetTestDataY,

        SetTrainSpeed,
        SetLoss,
        SetAccuracy,
        SetFitOptimizer,
        SetLayers,
        SetOnUpdateCount,
        BuildModel,

        GetLayersInfo,
        UpdateLayersInfo,

        FitWithEval,
        FitWithoutEval,

        TakeFitInfo,
        TakeLayerInfo,
        ModelBuilded,
        FitEnded,
        UpdateInfoEnded

    }
}
