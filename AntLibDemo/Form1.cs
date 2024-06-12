using Accord.Math;
using AntLib.Model;
using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Tools;

namespace AntLibDemo
{
    public partial class Form1 : Form
    {
        private DataArray[] _xTrain;
        private DataArray[] _yTrain;
        private DataArray[] _xTest;
        private DataArray[] _yTest;

        private IModel _autoEnc;
        private IModel _enc;
        private IModel _dec;

        ModelBuilder _modelBuilder;

        public Form1()
        {
            InitializeComponent();
            (_xTrain, _yTrain) = ParseData("../../../../mnist_train.csv", 3000);
            (_xTest, _yTest) = ParseData("../../../../mnist_test.csv", 1000);
            BuildAutoEnc();
            pictureBox1.Image = BitmapConverter.FloatArrayToBitmap(_xTrain[0].GetArray1D(), 28, 10);
        }

        private void BuildAutoEnc()
        {
            _modelBuilder = new ModelBuilder();
            _modelBuilder.SetLoss(Loss.AbsoluteLoss);
            _modelBuilder.SetAccuracy(Accuracy.MaxElementAccuracy);
            _modelBuilder.SetFitOptimizer(new NonFitOptimizerParam());

            _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new NonLayerOptimizerParam(784, 200));
            _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new NonLayerOptimizerParam(200, 100));
            _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new NonLayerOptimizerParam(100, 200));
            _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new NonLayerOptimizerParam(200, 784));

            Console.WriteLine(_modelBuilder.GetSummaryInfo());

            _autoEnc = _modelBuilder.BuildParallelModel(Environment.ProcessorCount);

            _autoEnc.SetOnUpdate((x, i) => { Console.WriteLine(x.ToString()); });
            _autoEnc.SetUpdateCount(10000);
        }

        private void BuildEncAndDec()
        {
            List<ILayerInfo> layerInfos = new List<ILayerInfo>(_autoEnc.GetLayersInfo());
            _modelBuilder.SetLayers(layerInfos.GetRange(0, 2).ToArray());
            _enc = _modelBuilder.BuildModel();

            _modelBuilder.SetLayers(layerInfos.GetRange(2, 2).ToArray());
            _dec = _modelBuilder.BuildModel();
        }

        private (DataArray[], DataArray[]) ParseData(string path, int count)
        {
            string[] lines = File.ReadAllText(path).Split(Environment.NewLine).RemoveAt(0);
            return Parser.ParseData(lines, count);
        }


        private void trainButton_Click(object sender, EventArgs e)
        {
            _autoEnc.Fit(_xTrain, _yTrain, 0.01f, 20);
            BuildEncAndDec();
        }

        private void encodeButton_Click(object sender, EventArgs e)
        {
            int examplePosition = StaticRandom.GetNextInt(0, _xTest.Length);
            pictureBox1.Image = BitmapConverter.FloatArrayToBitmap(_xTest[examplePosition].GetArray1D(), 28, 10);
            encodeTextBox.Text = String.Join(' ', _enc.Predict(_xTest[examplePosition]).GetArray1D());
            decodeTextBox.Text = encodeTextBox.Text;
        }

        private void decodeButton_Click(object sender, EventArgs e)
        {
            string[] textToDecode = encodeTextBox.Text.Split(" ");
            float[] arrayToDecode = textToDecode.Select(x => Convert.ToSingle(x)).ToArray();
            pictureBox2.Image = BitmapConverter.FloatArrayToBitmap(_dec.Predict(new DataArray(arrayToDecode)).GetArray1D(), 28, 10);
        }
    }
}