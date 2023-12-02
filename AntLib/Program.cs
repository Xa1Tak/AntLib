using Accord.Math;
using AntLib.Model;
using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Model.Tools;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World1!");
        ModelBuilder modelBuilder = new ModelBuilder();
        modelBuilder.SetLoss(Loss.AbsoluteLoss);
        modelBuilder.SetAccuracy(Accuracy.MaxElementAccuracy);
        modelBuilder.SetFitOptimizer(new NonFitOptimizerParam());
        modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(100, 50));
        modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(50, 10));
        modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.SoftMax, new AdamParam(10, 10));
        Console.WriteLine(modelBuilder.GetSummaryInfo());

        Model model = modelBuilder.BuildModel();
        DataArray[] X = GetDataArrays(10000, 100);
        DataArray[] Y = GetDataArrays(10000, 10);

        model.OnUpdate = (x) => { Console.WriteLine($"Epoch: {x.Epoch} progress: {x.Progress}  acc: {x.TrainAccuracy}  loss:{x.TrainLoss}  elapsed:{x.Time}"); };
        model.SetUpdateCount(2000);

        model.Fit(X, Y, 0.001f, 100);

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
}