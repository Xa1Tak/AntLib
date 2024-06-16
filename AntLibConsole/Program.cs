using Accord.Math;
using AntLib.Model;
using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Tools;
using RIS4;

internal class Program
{
    //private static void Main(string[] args)
    //{
    //    Console.WriteLine("Hello, World1!");
    //    ModelBuilder modelBuilder = new ModelBuilder();
    //    modelBuilder.SetLoss(Loss.AbsoluteLoss);
    //    modelBuilder.SetAccuracy(Accuracy.MaxElementAccuracy);
    //    modelBuilder.SetFitOptimizer(new NonFitOptimizerParam());
    //    modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(13, 100));
    //    modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(100, 100));
    //    modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Relu, new AdamParam(100, 2));
    //    Console.WriteLine(modelBuilder.GetSummaryInfo());

    //    Model model = modelBuilder.BuildModel();
    //    //DataArray[] X = GetDataArrays(10000, 100);
    //    //DataArray[] Y = GetDataArrays(10000, 10);
    //    float[][] xTrain = null;
    //    float[][] yTrain = null;
    //    float[][] xTest = null;
    //    float[][] yTest = null;
    //    //Parser.ParseForGenetic(out xTrain, out yTrain, out xTest, out yTest, "StudentsPerformance_with_headers.csv");
    //    Parser.ParseForNN2(out xTrain, out yTrain, out xTest, out yTest, "restaurants.csv");
    //    DataArray[] X = xTrain.Apply(x => new DataArray(x));
    //    DataArray[] Y = yTrain.Apply(x => new DataArray(x));

    //    DataArray[] Xtest = xTest.Apply(x => new DataArray(x));
    //    DataArray[] Ytest = yTest.Apply(x => new DataArray(x));

    //    model.OnUpdate = (x) => { Console.WriteLine($"Epoch: {x.Epoch} progress: {x.Progress}  acc: {x.TrainAccuracy}  loss:{x.TrainLoss}  elapsed:{x.Time}"); };
    //    model.SetUpdateCount(900);

    //    model.Fit(X, Y, 0.001f, 200);

    //    Console.WriteLine("Eval");

    //    for(int i = 0; i < Xtest.Length; i++)
    //    {
    //        Console.WriteLine($"#{i} pred {String.Join(" ", model.Predict(Xtest[i]).GetArray1D())}      actual {String.Join(" ", Ytest[i].GetArray1D())}");
    //    }

    //    FitInfo info = model.Evaluate(Xtest, Ytest);
    //    Console.WriteLine($"acc: {info.TestAccuracy}  loss:{info.TestLoss}  elapsed:{info.Time}");

    //    Console.ReadKey();
    //}

    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World1!");
        ModelBuilder modelBuilder = new ModelBuilder();

        modelBuilder.SetLoss(Loss.AbsoluteLoss);
        modelBuilder.SetAccuracy(Accuracy.MaxElementAccuracy);
        modelBuilder.SetFitOptimizer(new NonFitOptimizerParam());

        modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(13, 100));
        modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(100, 100));
        modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Relu, new AdamParam(100, 2));

        Console.WriteLine(modelBuilder.GetSummaryInfo());

        IModel model = modelBuilder.BuildParallelModel(Environment.ProcessorCount);
        //IModel model = modelBuilder.BuildModel();
        //DataArray[] X = GetDataArrays(10000, 100);
        //DataArray[] Y = GetDataArrays(10000, 10);

        //string wtf = model1.ToJson();

        //IModel model = modelBuilder.BuildParallelModel(2, wtf);
        //var a = modelBuilder.GetLayers();

        float[][] xTrain = null;
        float[][] yTrain = null;
        float[][] xTest = null;
        float[][] yTest = null;
        //Parser.ParseForGenetic(out xTrain, out yTrain, out xTest, out yTest, "StudentsPerformance_with_headers.csv");
        Parser.ParseForNN2(out xTrain, out yTrain, out xTest, out yTest, "../../../../restaurants.csv");
        DataArray[] X = xTrain.Apply(x => new DataArray(x));
        DataArray[] Y = yTrain.Apply(x => new DataArray(x));

        DataArray[] Xtest = xTest.Apply(x => new DataArray(x));
        DataArray[] Ytest = yTest.Apply(x => new DataArray(x));

        model.SetOnUpdate((x, i) => { Console.WriteLine(x.ToString()); });
        model.SetUpdateCount(9000);

        FitInfo result = model.Fit(X, Y, 0.001f, 200);
        Console.WriteLine(result.ToString());

        Console.WriteLine("Eval");

        for (int i = 0; i < Xtest.Length; i++)
        {
            Console.WriteLine($"#{i} pred {String.Join(" ", model.Predict(Xtest[i]).GetArray1D())}      actual {String.Join(" ", Ytest[i].GetArray1D())}");
        }

        FitInfo info = model.Evaluate(Xtest, Ytest);
        Console.WriteLine(info.ToString());

        Console.ReadKey();
    }

    private static DataArray[] GetDataArrays(int count, int oneLength)
    {
        DataArray[] dataArrays = new DataArray[count];
        for(int i = 0; i < dataArrays.Length; i++)
        {
            dataArrays[i] = new DataArray(Vector.Random(oneLength, 0f, 1f));
        }
        return dataArrays;
    }

    private IModel BuildLogisticregression(int numInputParams, int numOutputClassesuts)
    {
        ModelBuilder modelBuilder = new ModelBuilder();
        modelBuilder.SetLoss(Loss.AbsoluteLoss);
        modelBuilder.SetAccuracy(Accuracy.MaxElementAccuracy);
        modelBuilder.SetFitOptimizer(new NonFitOptimizerParam());
        modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(numInputParams, numOutputClassesuts));
        return modelBuilder.BuildModel();
    }
}