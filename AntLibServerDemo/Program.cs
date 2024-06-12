using Accord.Math;
using AntLib.Communication.Client;
using AntLib.Communication.Server;
using AntLib.Model;
using AntLib.Model.FitOptimizer;
using AntLib.Model.Layer.LayerOptimizer;
using AntLib.Model.ModelAccuracy;
using AntLib.Model.ModelLoss;
using AntLib.Tools;
using AntLibServerDemo;

internal class Program
{
    private static ModelBuilder _modelBuilder;
    private static DataArray[] _xTrain;
    private static DataArray[] _yTrain;
    private static DataArray[] _xTest;
    private static DataArray[] _yTest;
    private static void Main(string[] args)
    {
        string ip = "127.0.0.1";
        int port = 1515;
        string serverName = "AntLibDemoServer";
        BuildBuilder();
        (_xTrain, _yTrain) = ParseData("../../../../mnist_train.csv", 3000);
        (_xTest, _yTest) = ParseData("../../../../mnist_test.csv", 1000);

        AntLibServer server = new AntLibServer(serverName, ip, port);
        server.SetTrainData(_xTrain, _yTrain, _xTest, _yTest);
        server.SetModel(_modelBuilder, 0.001f);
        server.OnMessageReceive += x => Console.WriteLine(x);

        Console.WriteLine($"Hello, World! I am {serverName}. Model params:");
        Console.WriteLine(_modelBuilder.GetSummaryInfo());
        Console.WriteLine("Press Enter when all clients connected for 20 epoch");
        Console.ReadKey();
        Console.WriteLine("Working");
        server.Fit(20);

        Console.ReadKey();
    }

    private static void BuildBuilder()
    {
        _modelBuilder = new ModelBuilder();
        _modelBuilder.SetLoss(Loss.AbsoluteLoss);
        _modelBuilder.SetAccuracy(Accuracy.MaxElementAccuracy);
        _modelBuilder.SetFitOptimizer(new NonFitOptimizerParam());

        _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(784, 200));
        _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(200, 100));
        _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(100, 200));
        _modelBuilder.AddDense(AntLib.Model.Layer.ActivationFunction.ActivationFunc.Sigmoid, new AdamParam(200, 784));
    }

    private static (DataArray[], DataArray[]) ParseData(string path, int count)
    {
        string[] lines = File.ReadAllText(path).Split(Environment.NewLine).RemoveAt(0);
        return Parser.ParseData(lines, count);
    }
}