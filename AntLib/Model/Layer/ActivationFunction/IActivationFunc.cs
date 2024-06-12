using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer.ActivationFunction
{
    internal interface IActivationFunc
    {
        public DataArray Forward(DataArray data);
        public DataArray Backward(DataArray data);
        public string GetName();

        public IActivationFunc Clone();
    }
}
