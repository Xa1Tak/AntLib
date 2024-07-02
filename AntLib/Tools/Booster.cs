using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
using ILGPU.Runtime.OpenCL;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLib.Tools
{
    public static class Booster
    {
        private static bool _isBoosted = false;
        private static Context _context;
        internal static Accelerator _accelerator;

        public static void DoBoost(Device device)
        {
            _isBoosted = true;
            //_accelerator = device.CreateAccelerator(_context);
            _accelerator = _context.CreateCudaAccelerator(0);
        }

        public static Context.DeviceCollection<CLDevice> GetOpenCLDevices()
        {
            TryCreateContext();
            return _context.GetCLDevices();
        }

        public static Context.DeviceCollection<CudaDevice> GetCudaDevices()
        {
            TryCreateContext();
            return _context.GetCudaDevices();
        }

        public static ImmutableArray<Device> GetAllDevices()
        {
            TryCreateContext();
            return _context.Devices;
        }

        internal static bool IsBoosted()
        {
            return _isBoosted;
        }

        internal static MemoryBuffer1D<float, Stride1D.Dense> GetMemoryBuffer1D(int Length)
        {
            return _accelerator.Allocate1D<float>(Length);
        }

        internal static MemoryBuffer2D<float, Stride2D.DenseY> GetMemoryBuffer2D(int XLength, int YLength)
        {
            return _accelerator.Allocate2DDenseY<float>(new Index2D(XLength, YLength));
        }

        internal static MemoryBuffer3D<float, Stride3D.DenseXY> GetMemoryBuffer3D(int XLength, int YLength, int ZLength)
        {
            return _accelerator.Allocate3DDenseXY<float>(new Index3D(XLength, YLength, ZLength));
        }

        internal static void Sync()
        {
            if(_accelerator != null)
            {
                _accelerator.DefaultStream.Synchronize();
            }
        }

        internal static void GenCode()
        {
            _context.BeginCodeGeneration().Optimize();
        }

        private static void TryCreateContext()
        {
            if (_context == null)
            {
                _context = Context.Create(builder => builder.Default().EnableAlgorithms().Math(MathMode.Fast).Release());
            }
        }

    }
}
