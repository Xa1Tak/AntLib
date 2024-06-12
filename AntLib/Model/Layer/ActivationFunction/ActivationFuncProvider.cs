using AntLib.Model.Layer.Optimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.ActivationFunction
{
    public enum ActivationFunc
    {
        Relu,
        Sigmoid,
        SoftMax
    }

    internal static class ActivationFuncProvider
    {
        internal static IActivationFunc GetActivationFunc(int id, float[] param)
        {
            switch (id)
            {
                case (int)ActivationFunc.Relu:
                    {
                        return new Relu();
                    }
                case (int)ActivationFunc.Sigmoid:
                    {
                        return new Sigmoid();
                    }
                case (int)ActivationFunc.SoftMax:
                    {
                        return new SoftMax();
                    }
            }
            return null;
        }
    }
}
