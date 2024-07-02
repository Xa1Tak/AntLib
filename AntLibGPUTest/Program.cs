using System;
using System.Diagnostics;
using Accord.Math;
using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;


internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        using var context = Context.CreateDefault();
        foreach(var dev  in context.Devices)
        {
            Console.WriteLine(dev.Name);
        }
        int input = 200;
        int output = 200;
        float[,] matrix = Matrix.Random(input, output, -1f, 1f);
        float[] vector = Vector.Random(input, -1f, 1f);

        Console.WriteLine("ILGPUFit");
        var ilgpuResult = CheckILGPUFit(matrix, vector);
        Console.WriteLine(ilgpuResult.Item1);
        //Console.WriteLine(String.Join(" ",ilgpuResult.Item2));
        Console.WriteLine("-------------------------------------");

        Console.WriteLine("ILGPU");
        ilgpuResult = CheckILGPU(matrix, vector);
        Console.WriteLine(ilgpuResult.Item1);
        //Console.WriteLine(String.Join(" ",ilgpuResult.Item2));
        Console.WriteLine("-------------------------------------");

        Console.WriteLine("AccordFit");
        var accordResult = CheckAccordFit(matrix, vector);
        Console.WriteLine(accordResult.Item1);
        //Console.WriteLine(String.Join(" ", accordResult.Item2));
        Console.WriteLine("-------------------------------------");

        Console.WriteLine("Accord");
        accordResult = CheckAccord(matrix, vector);
        Console.WriteLine(accordResult.Item1);
        //Console.WriteLine(String.Join(" ", accordResult.Item2));
        Console.WriteLine("-------------------------------------");

        Console.ReadKey();
    }

    private static (double, float[]) CheckAccord(float[,] matrix, float[] vector)
    {
        float[] result = { 0 };
        result = vector.Dot(matrix);
        var sw = Stopwatch.StartNew();
        result = vector.Dot(matrix);
        //result = matrix.Dot(vector);
        sw.Stop();
        return (sw.Elapsed.TotalSeconds, result);
    }

    private static (double, float[]) CheckAccordFit(float[,] matrix, float[] vector)
    {
        float trainSpeed = 0.001f;
        float _e = 0.0000001f;
        float _b1 = 0.9f;
        float _b2 = 0.999f;
        float _bb2 = 1 - _b2;
        float _bb1 = 1 - _b1;
        float bb1Pow = 1 - (float)Math.Pow(_b1, 1);
        float bb2Pow = 1 - (float)Math.Pow(_b2, 1);
        float[] _nextGrad = new float[matrix.GetLength(0)];
        float[] _correction = new float[matrix.GetLength(1)];
        float[] _correction2 = new float[matrix.GetLength(1)];
        float[] e = Vector.Random(matrix.GetLength(1), 0f, 1f);
        float[] input = vector;
        float[] output = Vector.Random(matrix.GetLength(1), 0f, 1f);
        float[] _bias = new float[matrix.GetLength(1)];
        float[] _vtBias = new float[matrix.GetLength(1)];
        float[] _mtBias = new float[matrix.GetLength(1)];
        float[,] _vtWeights = new float[matrix.GetLength(0), matrix.GetLength(1)];
        float[,] _mtWeights = new float[matrix.GetLength(0), matrix.GetLength(1)];

        for (int i = 0; i < _correction.Length; i++)
        {
            _correction[i] = e[i] * output[i];
            _correction2[i] = _correction[i] * _correction[i];
            _vtBias[i] = _b2 * _vtBias[i] + (_bb2 * _correction2[i]);
            _mtBias[i] = _b1 * _mtBias[i] + (_bb1 * _correction[i]);
            _bias[i] -= trainSpeed * ((_mtBias[i] / bb1Pow) / ((float)Math.Sqrt((_vtBias[i] / bb2Pow) + _e)));

            for (int k = 0; k < matrix.GetLength(0); k++)
            {
                _nextGrad[k] += matrix[k, i] * _correction[i];
                _vtWeights[k, i] = _b2 * _vtWeights[k, i] + (_bb2 * _correction2[i] * input[k] * input[k]);
                _mtWeights[k, i] = _b1 * _mtWeights[k, i] + (_bb1 * _correction[i] * input[k]);
                matrix[k, i] -= trainSpeed * ((_mtWeights[k, i] / bb1Pow) / ((float)Math.Sqrt((_vtWeights[k, i] / bb2Pow) + _e)));
            }
        }
        output = Vector.Random(matrix.GetLength(1), 0f, 1f);
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < _correction.Length; i++)
        {
            _correction[i] = e[i] * output[i];
            _correction2[i] = _correction[i] * _correction[i];
            _vtBias[i] = _b2 * _vtBias[i] + (_bb2 * _correction2[i]);
            _mtBias[i] = _b1 * _mtBias[i] + (_bb1 * _correction[i]);
            _bias[i] -= trainSpeed * ((_mtBias[i] / bb1Pow) / ((float)Math.Sqrt((_vtBias[i] / bb2Pow) + _e)));

            for (int k = 0; k < matrix.GetLength(0); k++)
            {
                _nextGrad[k] += matrix[k, i] * _correction[i];
                _vtWeights[k, i] = _b2 * _vtWeights[k, i] + (_bb2 * _correction2[i] * input[k] * input[k]);
                _mtWeights[k, i] = _b1 * _mtWeights[k, i] + (_bb1 * _correction[i] * input[k]);
                matrix[k, i] -= trainSpeed * ((_mtWeights[k, i] / bb1Pow) / ((float)Math.Sqrt((_vtWeights[k, i] / bb2Pow) + _e)));
            }
        }
        sw.Stop();
        return (sw.Elapsed.TotalSeconds, _nextGrad);
    }

    private static (double, float[]) CheckILGPU(float[,] maxtrix, float[] vector)
    {
        using var context = Context.Create(builder => builder.Default().EnableAlgorithms().Math(MathMode.Fast));
        using var accelerator = context.Devices.Last().CreateAccelerator(context);
        var kernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView2D<float, Stride2D.DenseX>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>>(
                MatrixMultiplyAcceleratedKernel);
        using var matrixBuffer = accelerator.Allocate2DDenseX<float>(new Index2D(maxtrix.GetLength(0), maxtrix.GetLength(1)));
        using var vectorBuffer = accelerator.Allocate1D<float>(vector.Length);
        using var resultBuffer = accelerator.Allocate1D<float>(maxtrix.GetLength(1));
        kernel(resultBuffer.IntExtent, matrixBuffer.View, vectorBuffer.View, resultBuffer.View);
        matrixBuffer.CopyFromCPU(maxtrix);
        vectorBuffer.CopyFromCPU(vector);
        var sw = Stopwatch.StartNew();
        kernel(resultBuffer.IntExtent, matrixBuffer.View, vectorBuffer.View, resultBuffer.View);
        sw.Stop();
        float[] result = new float[maxtrix.GetLength(1)];
        resultBuffer.CopyToCPU(result);
        return (sw.Elapsed.TotalSeconds, result);
    }

    private static (double, float[]) CheckILGPUFit(float[,] maxtrix, float[] vector)
    {
        using var context = Context.Create(builder => builder.Default().EnableAlgorithms().Math(MathMode.Fast));
        using var accelerator = context.Devices.Last().CreateAccelerator(context);
        var kernel = accelerator.LoadAutoGroupedStreamKernel<
                Index1D,
                ArrayView2D<float, Stride2D.DenseX>,
                ArrayView2D<float, Stride2D.DenseX>,
                ArrayView2D<float, Stride2D.DenseX>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>,
                ArrayView1D<float, Stride1D.Dense>>(
                AdamFitAcceleratedKernel);
        using var weightsBuffer = accelerator.Allocate2DDenseX<float>(new Index2D(maxtrix.GetLength(0), maxtrix.GetLength(1)));
        using var vtweightsBuffer = accelerator.Allocate2DDenseX<float>(new Index2D(maxtrix.GetLength(0), maxtrix.GetLength(1)));
        using var mtweightsBuffer = accelerator.Allocate2DDenseX<float>(new Index2D(maxtrix.GetLength(0), maxtrix.GetLength(1)));
        using var inputBuffer = accelerator.Allocate1D<float>(vector.Length);
        using var nextGradBuffer = accelerator.Allocate1D<float>(vector.Length);
        using var outputBuffer = accelerator.Allocate1D<float>(maxtrix.GetLength(1));
        using var errorBuffer = accelerator.Allocate1D<float>(maxtrix.GetLength(1));
        using var biasBuffer = accelerator.Allocate1D<float>(maxtrix.GetLength(1));
        using var vtbiasBuffer = accelerator.Allocate1D<float>(maxtrix.GetLength(1));
        using var mtbiasBuffer = accelerator.Allocate1D<float>(maxtrix.GetLength(1));
        kernel(biasBuffer.IntExtent,
            weightsBuffer.View,
            mtweightsBuffer.View,
            vtweightsBuffer.View,
            biasBuffer.View,
            mtbiasBuffer.View,
            vtbiasBuffer.View,
            errorBuffer.View,
            inputBuffer.View,
            outputBuffer.View,
            nextGradBuffer.View);
        weightsBuffer.CopyFromCPU(maxtrix);
        inputBuffer.CopyFromCPU(vector);
        var sw = Stopwatch.StartNew();
        kernel(biasBuffer.IntExtent, 
            weightsBuffer.View, 
            mtweightsBuffer.View,
            vtweightsBuffer.View, 
            biasBuffer.View,
            mtbiasBuffer.View,
            vtbiasBuffer.View,
            errorBuffer.View,
            inputBuffer.View,
            outputBuffer.View,
            nextGradBuffer.View);
        sw.Stop();
        float[] result = new float[maxtrix.GetLength(0)];
        nextGradBuffer.CopyToCPU(result);
        return (sw.Elapsed.TotalSeconds, result);
    }

    static void MatrixMultiplyAcceleratedKernel(Index1D index, ArrayView2D<float, Stride2D.DenseX> matrixView, ArrayView1D<float, Stride1D.Dense> vectorView, ArrayView1D<float, Stride1D.Dense> resultView)
    {
        var x = index.X;
        //var y = index.Y;

        for(int i = 0; i < vectorView.Length; i++)
        resultView[x] += matrixView[i,x] * vectorView[i];
    }

    static void AdamFitAcceleratedKernel(Index1D index, 
        ArrayView2D<float, Stride2D.DenseX> weightsView,
        ArrayView2D<float, Stride2D.DenseX> mtweightsView,
        ArrayView2D<float, Stride2D.DenseX> vtweightsView,
        ArrayView1D<float, Stride1D.Dense> biasView,
        ArrayView1D<float, Stride1D.Dense> mtbiasView,
        ArrayView1D<float, Stride1D.Dense> vtbiasView,
        ArrayView1D<float, Stride1D.Dense> errorView,
        ArrayView1D<float, Stride1D.Dense> inputView,
        ArrayView1D<float, Stride1D.Dense> outputView,
        ArrayView1D<float, Stride1D.Dense> nextGradView)
    {
        float trainSpeed = 0.001f;
        float _e = 0.0000001f;
        float _b1 = 0.9f;
        float _b2 = 0.999f;
        float _bb2 = 1 - _b2;
        float _bb1 = 1 - _b1;
        float bb1Pow = 1 - (float)Math.Pow(_b1, 1);
        float bb2Pow = 1 - (float)Math.Pow(_b2, 1);

        float _correction = errorView[index] * outputView[index];
        float _correction2 = _correction * _correction;
        vtbiasView[index] = _b2 * vtbiasView[index] + (_bb2 * _correction2);
        mtbiasView[index] = _bb1 * mtbiasView[index] + (_bb1 * _correction);
        biasView[index] -= trainSpeed * ((mtbiasView[index] / bb1Pow) / ((float)Math.Sqrt((vtbiasView[index] / bb2Pow) + _e)));

        for (int k = 0; k < inputView.Length; k++)
        {
            nextGradView[k] += weightsView[k, index] * _correction;
            vtweightsView[k, index] = _b2 * vtweightsView[k, index] + (_bb2 * _correction2 * inputView[k] * inputView[k]);
            mtweightsView[k, index] = _b1 * mtweightsView[k, index] + (_bb1 * _correction * inputView[k]);
            weightsView[k, index] -= trainSpeed * ((mtweightsView[k, index] / bb1Pow) / ((float)Math.Sqrt((vtweightsView[k, index] / bb2Pow) + _e)));
        }
    }
}