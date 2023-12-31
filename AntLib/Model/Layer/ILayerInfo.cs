﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.Layer
{
    public interface ILayerInfo
    {
        public string GetInfo();
        public string GetWeightsInfo();

        public string GetInputsCount();

        public string GetOutputsCount();

        public ILayer GetLayer();

        public string ToJson();
    }
}
