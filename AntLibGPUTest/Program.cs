using System;
using System.Diagnostics;
using Accord.Math;
using ILGPU;
using ILGPU.Algorithms;
using ILGPU.Runtime;
using ComputeSharp;

[ThreadGroupSize(DefaultThreadGroupSizes.X)]
[GeneratedComputeShaderDescriptor]
internal partial struct MultiplyByTwo(ReadWriteTexture2D<float> matrixBuffer, ReadWriteTexture1D<float> vectorBuffer, ReadWriteTexture1D<float> resultBuffer) : IComputeShader
{
    public void Execute()
    {
        resultBuffer[ThreadIds.Y] += matrixBuffer[ThreadIds.XY] * vectorBuffer[ThreadIds.X];
    }
}

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
        int input = 784;
        int output = 200;
        float[,] matrix = Matrix.Random(input, output, -1f, 1f);
        float[] vector = Vector.Random(input, -1f, 1f);

        Console.WriteLine("ILGPU");
        var ilgpuResult = CheckILGPU(matrix, vector);
        Console.WriteLine(ilgpuResult.Item1);
        //Console.WriteLine(String.Join(" ",ilgpuResult.Item2));
        Console.WriteLine("-------------------------------------");

        Console.WriteLine("Accord");
        var accordResult = CheckAccord(matrix, vector);
        Console.WriteLine(accordResult.Item1);
        //Console.WriteLine(String.Join(" ", accordResult.Item2));
        Console.WriteLine("-------------------------------------");

        Console.WriteLine("ComputeSharp");
        var computeResult = CheckComputeSharp(matrix, vector);
        Console.WriteLine(computeResult.Item1);
        //Console.WriteLine(String.Join(" ",computeResult.Item2));
        Console.ReadKey();
    }

    private static (long, float[]) CheckAccord(float[,] matrix, float[] vector)
    {
        float[] result = { 0 };
        var sw = Stopwatch.StartNew();
        result = vector.Dot(matrix);
        sw.Stop();
        return (sw.ElapsedTicks, result);
    }

    private static (long, float[]) CheckComputeSharp(float[,] matrix, float[] vector)
    {
        float[] result = new float[matrix.GetLength(1)];
        using ReadWriteTexture2D<float> matrixBuffer = GraphicsDevice.GetDefault().AllocateReadWriteTexture2D(matrix);
        using ReadWriteTexture1D<float> vectorBuffer = GraphicsDevice.GetDefault().AllocateReadWriteTexture1D(vector);
        using ReadWriteTexture1D<float> resultBuffer = GraphicsDevice.GetDefault().AllocateReadWriteTexture1D(result);

        var kernel = new MultiplyByTwo(matrixBuffer, vectorBuffer, resultBuffer);

        var sw = Stopwatch.StartNew();
        GraphicsDevice.GetDefault().For(matrix.GetLength(0), matrix.GetLength(1), kernel);
        sw.Stop();
        resultBuffer.CopyTo(result);
        return (sw.ElapsedTicks, result);
    }

    private static (long, float[]) CheckILGPU(float[,] maxtrix, float[] vector)
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
        matrixBuffer.CopyFromCPU(maxtrix);
        vectorBuffer.CopyFromCPU(vector);
        var sw = Stopwatch.StartNew();
        kernel(resultBuffer.IntExtent, matrixBuffer.View, vectorBuffer.View, resultBuffer.View);
        sw.Stop();
        float[] result = new float[maxtrix.GetLength(1)];
        resultBuffer.CopyToCPU(result);
        return (sw.ElapsedTicks, result);
    }

    static void MatrixMultiplyAcceleratedKernel(Index1D index, ArrayView2D<float, Stride2D.DenseX> matrixView, ArrayView1D<float, Stride1D.Dense> vectorView, ArrayView1D<float, Stride1D.Dense> resultView)
    {
        var x = index.X;
        //var y = index.Y;

        for(int i = 0; i < vectorView.Length; i++)
        resultView[x] += matrixView[i,x] * vectorView[i];
    }
}