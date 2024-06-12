using AntLib.Model.ModelAccuracy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelLoss
{
    public enum Loss
    {
        AbsoluteLoss,
        SquaredLoss
    }
    internal static class LossProvider
    {
        internal static ILoss GetLoss(Loss loss, float[] param)
        {
            switch (loss)
            {
                case Loss.AbsoluteLoss:
                    {
                        return new AbsoluteLoss();
                    }
                break;
                case Loss.SquaredLoss:
                    {
                        return new SquaredError();
                    }
                break;
            }
            return null;
        }
    }
}
