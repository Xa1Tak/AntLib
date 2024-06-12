using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.IO;
using Accord.Math;
using AntLib.Tools;

namespace AntLibServerDemo
{
    public static class Parser
    {
        public static (DataArray[], DataArray[]) ParseData(string[] lines, int count)
        {
            DataArray[] resultX = new DataArray[count];
            DataArray[] resultY = new DataArray[count];
            float[] tempX;
            float[] tempY;
            for(int i = 0; i < count; i++)
            {
                (tempX, tempY) = ParseLine(lines[i]);
                resultX[i] = new DataArray(tempX);
                resultY[i] = new DataArray(tempY);
            }
            return (resultX, resultY);
        }

        private static (float[], float[]) ParseLine(string line)
        {
            string[] splitedLine = line.Split(',');
            float[] resultX = new float[line.Length - 1];
            float[] resultY = new float[line.Length - 1];
            resultX = splitedLine.RemoveAt(0).Select(x => Convert.ToSingle(x)).ToArray().Divide(255f);
            resultY = resultX.DeepClone();
            return (resultX, resultY);
        }
    }
}
