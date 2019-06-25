using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace FEZSkillCounter
{
    public class FEZScreenShot : IDisposable
    {
        public Bitmap Image { get; }
        public long TimeStamp { get; }

        public FEZScreenShot(Bitmap image, long timeStamp)
        {
            Image     = image;
            TimeStamp = timeStamp;
        }

        public void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
            }
        }
    }

    public class FEZScreenShooter
    {
        public FEZScreenShot Shoot()
        {
            var tick = DateTime.Now.Ticks;

            using (var p = Process.GetProcessesByName("FEzero_Client").FirstOrDefault())
            {
                if (p == null)
                {
                    return new FEZScreenShot(null, tick);
                }

                if (!NativeMethods.GetWindowRect(p.MainWindowHandle, out RECT rect))
                {
                    return new FEZScreenShot(null, tick);
                }

                var size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                var bmp  = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    var hdc = NativeMethods.GetDC(p.MainWindowHandle);

                    NativeMethods.BitBlt(g.GetHdc(), 0, 0, size.Width, size.Height, hdc, 0, 0, NativeMethods.TernaryRasterOperations.SRCCOPY);

                    NativeMethods.ReleaseDC(p.MainWindowHandle, hdc);
                }

                return new FEZScreenShot(bmp, tick);
            }
        }
    }
}
