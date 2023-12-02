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
        internal static IAccuracy GetAccuracy(int id, float[] param)
        {
            switch (id)
            {
                case (int)Accuracy.MaxElementAccuracy:
                    {
                        return new MaxElementAccuracy();
                    }
            }
            return null;
        }
    }
}
