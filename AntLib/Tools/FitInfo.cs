using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ILGPU.IR.Analyses.Uniforms;

namespace AntLib.Tools
{
    public struct FitInfo
    {
        internal float Time;
        internal float TrainAccuracy;
        internal float TrainLoss;
        internal float TestAccuracy;
        internal float TestLoss;
        internal float Progress;
        internal int Epoch;

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

        internal FitInfo Divide(int div)
        {
            FitInfo result = new FitInfo();
            result.Time = Time;
            result.Epoch = Epoch;
            result.Progress = Progress;
            result.TrainAccuracy = TrainAccuracy / div;
            result.TrainLoss = TrainLoss / div;
            result.TestAccuracy = TestAccuracy / div;
            result.TestLoss = TestLoss / div;
            return result;
        }

        public override string ToString()
        {
            return $"Epoch: {Epoch} progress: {Progress}  acc: {TrainAccuracy}  loss:{TrainLoss}  evalAcc:{TestAccuracy}  evalLoss:{TestLoss}  elapsed:{Time}";
        }

        public static FitInfo operator +(FitInfo info1, FitInfo info2)
        {
            FitInfo result = new FitInfo();
            result.Progress = info2.Progress;
            result.Epoch = info2.Epoch;
            result.Time = info1.Time + info2.Time;
            result.TrainAccuracy = info1.TrainAccuracy + info2.TrainAccuracy;
            result.TrainLoss = info1.TrainLoss + info2.TrainLoss;
            result.TestAccuracy = info1.TestAccuracy + info2.TestAccuracy;
            result.TestLoss = info1.TestLoss + info2.TestLoss;
            return result;
        }
    }
}
