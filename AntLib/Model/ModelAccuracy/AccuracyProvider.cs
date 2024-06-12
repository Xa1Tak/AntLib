using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelAccuracy
{
    public enum Accuracy
    {
        MaxElementAccuracy = 1
    }

    public static class AccuracyProvider
    {
        internal static IAccuracy GetAccuracy(Accuracy accuracy, float[] param)
        {
            switch (accuracy)
            {
                case Accuracy.MaxElementAccuracy:
                    {
                        return new MaxElementAccuracy();
                    }
            }
            return null;
        }
    }
}
