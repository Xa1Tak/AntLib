using Accord.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntLibDemo
{
    internal static class BitmapConverter
    {
        public static Bitmap FloatArrayToBitmap(float[] array, int sideLength, int scale)
        {
            byte[,,] tensor = ArrayToByteTensor(array, sideLength);
            Bitmap pic = ConvertByteTensorToImageUnsafe(tensor);

            Bitmap result = new Bitmap(sideLength * scale, sideLength * scale);
            using (var gr = Graphics.FromImage(result))
            {
                gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                gr.DrawImage(pic, 0, 0, sideLength * scale, sideLength * scale);
            }

            return result;
        }

        private static byte[,,] ArrayToByteTensor(float[] array, int sideLength)
        {
            byte[,,] tensor = new byte[sideLength, sideLength, 3];
            for (int i = 0; i < tensor.GetLength(0); i++)
            {
                for (int k = 0; k < tensor.GetLength(1); k++)
                {
                    tensor[k, i, 0] = (byte)(array[i * sideLength + k] * 255);
                    tensor[k, i, 1] = (byte)(array[i * sideLength + k] * 255);
                    tensor[k, i, 2] = (byte)(array[i * sideLength + k] * 255);
                }
            }
            return tensor;
        }

        public static Bitmap ConvertByteTensorToImageUnsafe(byte[,,] tensor)
        {
            Bitmap image = new Bitmap(tensor.GetLength(0), tensor.GetLength(1), PixelFormat.Format24bppRgb);
            BitmapData bmd = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            unsafe
            {
                byte* row;
                for (int y = 0; y < bmd.Height; y++)
                {
                    row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    for (int x = 0; x < bmd.Width; x++)
                    {
                        *(row++) = tensor[x, y, 2];
                        *(row++) = tensor[x, y, 1];
                        *(row++) = tensor[x, y, 0];
                    }
                }
                image.UnlockBits(bmd);
            }
            return image;
        }
    }
}
