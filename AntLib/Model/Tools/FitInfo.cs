using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Tools
{
    public struct FitInfo
    {
        public float Time;
        public float TrainAccuracy;
        public float TrainLoss;
        public float TestAccuracy;
        public float TestLoss;
        public float Progress;
        public int Epoch;

        public FitInfo(float time, float trainAccuracy, float trainLoss, float testAccuracy, float testLoss, float progress, int epoch)
        {
            Time = time;
            TrainAccuracy = trainAccuracy;
            TrainLoss = trainLoss;
            TestAccuracy = testAccuracy;
            TestLoss = testLoss;
            Progress = progress;
            Epoch = epoch;
        }
    }
}
