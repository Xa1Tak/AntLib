using Accord.Math;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.ActivationFunction
{
    internal class SoftMax : IActivationFunc
    {
        public DataArray Forward(DataArray data)
        {
            if(data.GetDimensions() == 1)
            {
                return new DataArray(Special.Softmax(data.GetArray1D().Apply(x => (double)x)).Apply(x => (float)x));
                //return new DataArray(data.GetArray1D().Apply(x => (float)(1 / (1 + Math.Pow(Math.E, -x)))));
            }

            return data;

        }
        public DataArray Backward(DataArray data)
        {
            if(data.GetDimensions() == 1)
            {
                return new DataArray(data.GetArray1D().Apply(x => x * (1 - x)));
            }

            if(data.GetDimensions() == 2)
            {
                return new DataArray(data.GetArray2D().Apply(x => x * (1 - x)));
            }

            return data;
        }
        public string GetName()
        {
            return "SoftMax";
        }

        IActivationFunc IActivationFunc.Clone()
        {
            return new SoftMax();
        }
    }
}
