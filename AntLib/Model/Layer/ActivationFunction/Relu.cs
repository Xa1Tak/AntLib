using Accord.Math;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.ActivationFunction
{
    internal class Relu : IActivationFunc
    {
        DataArray IActivationFunc.Backward(DataArray data)
        {
            if (data.GetDimensions() == 1)
            {
                return new DataArray(data.GetArray1D().Apply(x =>
                {
                    if (x >= 0) return 1f;
                    return 0f;
                }));
            }

            if (data.GetDimensions() == 2)
            {
                return new DataArray(data.GetArray2D().Apply(x =>
                {
                    if (x >= 0) return 1f;
                    return 0f;
                })); ;
            }

            return data;
        }

        IActivationFunc IActivationFunc.Clone()
        {
            return new Relu();
        }

        DataArray IActivationFunc.Forward(DataArray data)
        {
            if (data.GetDimensions() == 1)
            {
                return new DataArray(data.GetArray1D().Apply(x => (float)(Math.Max(0, x))));
            }

            if (data.GetDimensions() == 2)
            {
                return new DataArray(data.GetArray2D().Apply(x => (float)(Math.Max(0, x))));
            }

            return data;
        }

        string IActivationFunc.GetName()
        {
            return "Relu";
        }
    }
}
