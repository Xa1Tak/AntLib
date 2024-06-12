using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Reader
{
    internal interface IReader
    {
        public void ReadData(string filePath, out string error, out DataArray[] X, out DataArray[] Y);
    }
}
