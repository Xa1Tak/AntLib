using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelAccuracy
{
    public interface IAccuracy
    {
        public void Reset();
        public float CalcAccuracy(DataArray YPred, DataArray YActual);
        public string GetName();
        public Accuracy GetAccuracy();
    }
}
