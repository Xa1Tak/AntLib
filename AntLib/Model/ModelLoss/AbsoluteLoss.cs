using Accord.Math;
using AntLib.Tools;
using ILGPU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelLoss
{
    internal class AbsoluteLoss : ILoss
    {
        public float CalcLoss(DataArray errorArray)
        {
            if (errorArray.GetDimensions() == 1)
            {
                return errorArray.GetArray1D().Apply(x => Math.Abs(x)).Sum();
            }

            if (errorArray.GetDimensions() == 2)
            {
                return errorArray.GetArray2D().Apply(x => IntrinsicMath.Abs(x)).Sum();
            }

            return 0;
        }
        
        public string GetName()
        {
            return "AbsoluteLoss";
        }

        Loss ILoss.GetLoss()
        {
            return Loss.AbsoluteLoss;
        }
    }
}
