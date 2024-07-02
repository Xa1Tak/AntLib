using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.Layer.Optimizer;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer
{
    public class Tester
    {
        public void TestAdam(DataArray data, int loopCount)
        {
            int length = Convert.ToInt32(data.GetShape());
            AdamParam param = new AdamParam(length, length);
            ILayerOptimizer adam = param.GetAdam();
            for (int i = 0; i < loopCount; i++)
            {
                adam.Forward(data);
            }
        }

        public void TestAdam(DataArray[] data, int loopCount)
        {
            int length = Convert.ToInt32(data[0].GetShape());
            AdamParam param = new AdamParam(length, length);
            ILayerOptimizer adam = param.GetAdam();
            for (int i = 0; i < loopCount; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    adam.Forward(data[j]);
                }
            }
        }

        public void TestAdamBack(DataArray data, int loopCount)
        {
            int length = Convert.ToInt32(data.GetShape());
            AdamParam param = new AdamParam(length, length);
            ILayerOptimizer adam = param.GetAdam();
            for (int i = 0; i < loopCount; i++)
            {
                adam.Backward(data, data, data, 1f, 1);
            }
        }

        public void TestAdamBack(DataArray[] data, int loopCount)
        {
            int length = Convert.ToInt32(data[0].GetShape());
            AdamParam param = new AdamParam(length, length);
            ILayerOptimizer adam = param.GetAdam();

            for (int i = 0; i < loopCount; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    adam.Backward(data[j], data[j], data[j], 1f, 1);
                }
            }
        }

        public void TestAdamFull(DataArray data, int loopCount)
        {
            int length = Convert.ToInt32(data.GetShape());
            AdamParam param = new AdamParam(length, length);
            ILayerOptimizer adam = param.GetAdam();
            DataArray output;
            for (int i = 0; i < loopCount; i++)
            {
                output = adam.Forward(data).Subtract(data);
                adam.Backward(output, output, output, 1f, 1);
            }
        }

        public void TestAdamFull(DataArray[] data, int loopCount)
        {
            int length = Convert.ToInt32(data[0].GetShape());
            AdamParam param = new AdamParam(length, length);
            ILayerOptimizer adam = param.GetAdam();
            DataArray output;
            for (int i = 0; i < loopCount; i++)
            {
                for (int j = 0; j < data.Length; j++)
                {
                    output = adam.Forward(data[j]);
                    adam.Backward(output, output, output, 1f, 1);
                }
            }
        }
    }
}
