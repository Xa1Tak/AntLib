using Accord.Math;
using AntLib.Model.Layer;
using AntLib.Model.Layer.LayerOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Tools
{
    internal static class ParallelModelHelper
    {
        internal static FitInfo CombineInfo(FitInfo[] infos)
        {
            int count = 0;
            FitInfo result = new FitInfo(0, 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Time != 0)
                {
                    count++;
                    result.Progress += infos[i].Progress;
                    result.TrainLoss += infos[i].TrainLoss;
                    result.TestLoss += infos[i].TestLoss;
                    result.TrainAccuracy += infos[i].TrainAccuracy;
                    result.Epoch += infos[i].Epoch;
                    result.Time += infos[i].Time;
                }
            }
            if(count == 0) return result;
            result.Epoch /= count;
            result.TrainAccuracy /= count;
            result.TestAccuracy /= count;
            result.TrainLoss /= count;
            result.TestLoss /= count;
            result.Time /= count;
            return result;
        }

        internal static ILayerInfo[] CombineModels(ILayerInfo[][] param)
        {
            ILayerInfo[] result = param[0];
            //Parallel.For(1, param.Length, (i) =>
            //{
            //    for (int k = 0; k < param[i].Length; k++)
            //    {
            //        result[k].Combine(param[i][k]);
            //    }
            //});
            for (int i = 1; i < param.Length; i++)
            {
                for (int k = 0; k < param[i].Length; k++)
                {
                    result[k].Combine(param[i][k]);
                }
            }
            return result;
        }


        internal static (DataArray[][] X, DataArray[][] Y) SplitData(DataArray[] X, DataArray[] Y, int threads)
        {
            DataArray[][] resultX = new DataArray[threads][];
            DataArray[][] resultY = new DataArray[threads][];
            int countPerThread = X.Length / threads;
            int countOverLastBound = X.Length - threads * countPerThread;
            resultX[threads - 1] = new DataArray[countPerThread + countOverLastBound];
            resultY[threads - 1] = new DataArray[countPerThread + countOverLastBound];
            for (int i = 0; i < threads; i++)
            {
                if (resultX[i] == null) resultX[i] = new DataArray[countPerThread];
                if (resultY[i] == null) resultY[i] = new DataArray[countPerThread];
                for (int k = i * countPerThread; k < i * countPerThread + resultX[i].Length; k++)
                {
                    int localIndex = k - i * countPerThread;
                    resultX[i][localIndex] = X[k];
                    resultY[i][localIndex] = Y[k];
                }
            }
            return (resultX, resultY);
        }
    }
}
