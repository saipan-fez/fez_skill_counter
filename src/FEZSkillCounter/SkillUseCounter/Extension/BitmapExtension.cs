using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace SkillUseCounter.Extension
{
    internal static class BitmapExtension
    {
        private static HashAlgorithm provider = SHA1.Create();

        public static string SHA1Hash(this Bitmap bitmap)
        {
            byte[] ret;
            var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var bitmapData    = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var pixels        = new byte[bitmapData.Width * bitmapData.Height * bytesPerPixel];
            Marshal.Copy(bitmapData.Scan0, pixels, 0, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            ret = provider.ComputeHash(pixels);

            return string.Join(",", ret);
        }

        public static unsafe void ToThresholding(this Bitmap bitmap, bool reverse = false)
        {
            BitmapData bitmapData = null;

            try
            {
                bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                ToThresholding(bitmapData, reverse);
            }
            finally
            {
                try
                {
                    if (bitmapData != null)
                    {
                        bitmap.UnlockBits(bitmapData);
                        bitmapData = null;
                    }
                }
                catch { }
            }
        }

        private static unsafe void ToThresholding(BitmapData bitmapData, bool reverse)
        {
            int bpp = 3;
            int stride = bitmapData.Stride;
            byte* startPtr = (byte*)bitmapData.Scan0;
            uint color;
            uint com_color = reverse ? (uint)0x000000 : (uint)0xFFFFFF;
            byte set_color = reverse ? (byte)0xFF : (byte)0x00;

            for (int y = 0; y < bitmapData.Height; y++)
            {
                byte* lptr = startPtr + stride * y;
                byte* rptr = lptr + bitmapData.Width * bpp;

                for (byte* ptr = lptr; ptr < rptr; ptr += bpp)
                {
                    color = (uint)((*ptr) | (*(ptr + 1) << 8) | (*(ptr + 2) << 16));

                    if (color != com_color)
                    {
                        *(ptr + 0) = set_color;
                        *(ptr + 1) = set_color;
                        *(ptr + 2) = set_color;
                    }
                }
            }
        }

        public static Bitmap FillPaddingToZero(this Bitmap bitmap)
        {
            var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var bitmapData    = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var pixels        = new byte[bitmapData.Width * bitmapData.Height * bytesPerPixel];

            for (int row = 0; row < bitmapData.Height; row++)
            {
                var dataBeginPointer = IntPtr.Add(bitmapData.Scan0, row * bitmapData.Stride);
                Marshal.Copy(dataBeginPointer, pixels, row * bitmapData.Width * bytesPerPixel, bitmapData.Width * bytesPerPixel);
            }

            Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);
            bitmap.UnlockBits(bitmapData);

            return bitmap;
        }

        public static byte[] ConvertToByteArray(this Bitmap bitmap)
        {
            var bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
            var bitmapData    = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            var pixels        = new byte[bitmapData.Width * bitmapData.Height * bytesPerPixel];

            for (int row = 0; row < bitmapData.Height; row++)
            {
                var dataBeginPointer = IntPtr.Add(bitmapData.Scan0, row * bitmapData.Stride);
                Marshal.Copy(dataBeginPointer, pixels, row * bitmapData.Width * bytesPerPixel, bitmapData.Width * bytesPerPixel);
            }

            bitmap.UnlockBits(bitmapData);

            return pixels;
        }
    }
}
