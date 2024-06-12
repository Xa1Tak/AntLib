using Accord.Math;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelLoss
{
    internal class SquaredError : ILoss
    {
        float ILoss.CalcLoss(DataArray errorArray)
        {
            if (errorArray.GetDimensions() == 1)
            {
                return errorArray.GetArray1D().Apply(x => x * x).Sum();
            }

            if (errorArray.GetDimensions() == 2)
            {
                return errorArray.GetArray2D().Apply(x => x * x).Sum();
            }

            return 0;
        }

        Loss ILoss.GetLoss()
        {
            return Loss.SquaredLoss;
        }

        string ILoss.GetName()
        {
            return "SquaredLoss";
        }
    }
}
