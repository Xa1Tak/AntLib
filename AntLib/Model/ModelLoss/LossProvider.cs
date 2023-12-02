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
        AbsoluteLoss = 1
    }
    internal static class LossProvider
    {
        internal static ILoss GetLoss(int id, float[] param)
        {
            switch (id)
            {
                case (int)Loss.AbsoluteLoss:
                    {
                        return new AbsoluteLoss();
                    }
            }
            return null;
        }
    }
}
