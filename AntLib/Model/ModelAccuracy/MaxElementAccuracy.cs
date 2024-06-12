using Accord.Math;
using AntLib.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Model.ModelAccuracy
{
    internal class MaxElementAccuracy : IAccuracy
    {
        private float _posPredictionsCount;
        private float _count;
        public MaxElementAccuracy() 
        {
            _posPredictionsCount = 0;
            _count = 0;
        }

        public void Reset()
        {
            _posPredictionsCount = 0;
            _count = 0;
        }

        public float CalcAccuracy(DataArray YPred, DataArray YActual)
        {
            if(YPred.GetDimensions() == 1 && YActual.GetDimensions() == 1)
            {
                _count++;
                int maxElemPred = YPred.GetArray1D().ArgMax();
                int maxElemActual = YActual.GetArray1D().ArgMax();
                if (maxElemPred == maxElemActual)
                {
                    _posPredictionsCount++;
                }
                return _posPredictionsCount / _count;
            }
            if (YPred.GetDimensions() == 2 && YActual.GetDimensions() == 2)
            {
                _count++;
                Tuple<int, int> maxElemPred = YPred.GetArray2D().ArgMax();
                Tuple<int, int> maxElemActual = YActual.GetArray2D().ArgMax();
                if (maxElemPred == maxElemActual) _posPredictionsCount++;
                return _posPredictionsCount / _count;
            }
            return _posPredictionsCount/_count;
        }

        public string GetName()
        {
            return "MaxElementAccuracy";
        }

        Accuracy IAccuracy.GetAccuracy()
        {
            return Accuracy.MaxElementAccuracy;
        }
    }
}
