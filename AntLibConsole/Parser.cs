using Accord.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RIS4
{
    internal class Parser
    {
        public static void ParseForPrecedent(out float[][] xTrain, out float[] yTrain, out float[][] xTest, out float[] yTest, string path)
        {
            string[] lines = File.ReadAllLines(path);
            List<string> line;
            xTrain = new float[lines.Length - 11][];
            yTrain = new float[lines.Length - 11];
            xTest = new float[10][];
            yTest = new float[10];
            int trainCount = lines.Length - 10;
            for (int i = 1; i < trainCount; i++)
            {
                line = new List<string>(lines[i].Split(","));
                line.RemoveAt(0);
                line.RemoveAt(line.Count - 1);
                line.RemoveAt(line.Count - 1);
                yTrain[i - 1] = float.Parse(line.Last());
                line.RemoveAt(line.Count - 1);
                xTrain[i - 1] = ParseFromString(line);
            }
            for (int i = trainCount; i < lines.Length; i++)
            {
                line = new List<string>(lines[i].Split(","));
                line.RemoveAt(0);
                line.RemoveAt(line.Count - 1);
                line.RemoveAt(line.Count - 1);
                yTest[i - trainCount] = float.Parse(line.Last());
                line.RemoveAt(line.Count - 1);
                xTest[i - trainCount] = ParseFromString(line);
            }
        }

        public static void ParseForGenetic(out float[][] xTrain, out float[] yTrain, out float[][] xTest, out float[] yTest, string path)
        {
            ParseForPrecedent(out xTrain, out yTrain, out xTest, out yTest, path);
            Normalize(xTrain, xTest);
        }

        public static void ParseForNN(out float[][] xTrain, out float[][] yTrain, out float[][] xTest, out float[][] yTest, string path)
        {
            float[] yTr = null;
            float[] yTs = null;
            ParseForPrecedent(out xTrain, out yTr, out xTest, out yTs, path);
            Normalize(xTrain, xTest);
            Normalize(yTr, yTs, out yTrain, out yTest);
        }

        private static void Normalize(float[][] xTrain, float[][] xTest)
        {
            float tempMaxColValues = 0;
            float trainTempMaxColValues = 0;
            float testTempMaxColValues = 0;

            for (int i = 0; i < xTrain[0].Length; i++)
            {
                trainTempMaxColValues = xTrain.GetColumn(i).Max();
                testTempMaxColValues = xTrain.GetColumn(i).Max();
                tempMaxColValues = Math.Max(trainTempMaxColValues, testTempMaxColValues);
                xTrain.Apply(x => x[i] /= tempMaxColValues);
                xTest.Apply(x => x[i] /= tempMaxColValues);
            }
        }

        private static void Normalize(float[] yTrain, float[] yTest, out float[][] yTr, out float[][] yTs)
        {
            yTr = new float[yTrain.Length][];
            yTs = new float[yTest.Length][];
            for(int i = 0; i < yTr.Length; i++)
            {
                if (yTrain[i] == 1f)
                {
                    yTr[i] = new float[] { 1,0,0,0 };
                }
                if (yTrain[i] == 2f)
                {
                    yTr[i] = new float[] { 0, 1, 0, 0 };
                }
                if (yTrain[i] == 3f)
                {
                    yTr[i] = new float[] { 0, 0, 1, 0 };
                }
                if (yTrain[i] == 4f)
                {
                    yTr[i] = new float[] { 0, 0, 0, 1 };
                }
            }

            for (int i = 0; i < yTs.Length; i++)
            {
                if (yTest[i] == 1f)
                {
                    yTs[i] = new float[] { 1, 0, 0, 0 };
                }
                if (yTest[i] == 2f)
                {
                    yTs[i] = new float[] { 0, 1, 0, 0 };
                }
                if (yTest[i] == 3f)
                {
                    yTs[i] = new float[] { 0, 0, 1, 0 };
                }
                if (yTest[i] == 4f)
                {
                    yTs[i] = new float[] { 0, 0, 0, 1 };
                }
            }
        }

        public static void ParseForNN2(out float[][] xTrain, out float[][] yTrain, out float[][] xTest, out float[][] yTest, string path)
        {
            string[] lines = File.ReadAllLines(path);
            List<string> line;
            xTrain = new float[lines.Length - 51][];
            yTrain = new float[lines.Length - 51][];
            xTest = new float[50][];
            yTest = new float[50][];
            int trainCount = lines.Length - 50;
            for (int i = 1; i < trainCount; i++)
            {
                line = new List<string>(lines[i].Split(";"));
                line.RemoveRange(0,4);
                line.RemoveAt(line.Count - 1);
                line.RemoveAt(line.Count - 1);
                yTrain[i - 1] = ParseY(float.Parse(line.First().Replace('.',',')));
                line.RemoveAt(0);
                xTrain[i - 1] = ParseFromString(line);
            }
            for (int i = trainCount; i < lines.Length; i++)
            {
                line = new List<string>(lines[i].Split(";"));
                line.RemoveRange(0, 4);
                line.RemoveAt(line.Count - 1);
                line.RemoveAt(line.Count - 1);
                yTest[i - trainCount] = ParseY(float.Parse(line.First().Replace('.', ',')));
                line.RemoveAt(0);
                xTest[i - trainCount] = ParseFromString(line);
            }
            Normalize(xTrain, xTest);
        }

        private static float[] ParseY(float item)
        {
            if(item < 0)
            {
                return new float[] { 0, -item };
            }
            return new float[] { item, 0 };
        }

        private static float[] ParseFromString(List<string> text)
        {
            float[] result = new float[text.Count];
            for (int i = 0; i < text.Count; i++)
            {
                result[i] = float.Parse(text[i].Replace('.', ','));
            }
            return result;
        }
    }
}
