using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelLoss
{
    public interface ILoss
    {
        public float CalcLoss(DataArray errorArray);
        public string GetName();
        public Loss GetLoss();
    }
}
