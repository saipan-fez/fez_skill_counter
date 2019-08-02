using SkillUseCounter.Common;
using SkillUseCounter.Entity;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace SkillUseCounter
{
    internal class FEZScreenShotStorage
    {
        public FEZScreenShot Shoot()
        {
            var timestamp = DateTime.Now.Ticks;

            using (var process = Process.GetProcessesByName("FEzero_Client").FirstOrDefault())
            {
                if (process == null)
                {
                    return new FEZScreenShot(null, timestamp);
                }

                if (!NativeMethods.GetWindowRect(process.MainWindowHandle, out RECT rect))
                {
                    return new FEZScreenShot(null, timestamp);
                }

                var size = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                var bmp  = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
                using (var graphics = Graphics.FromImage(bmp))
                {
                    var hdc = NativeMethods.GetDC(process.MainWindowHandle);

                    NativeMethods.BitBlt(graphics.GetHdc(), 0, 0, size.Width, size.Height, hdc, 0, 0, NativeMethods.TernaryRasterOperations.SRCCOPY);

                    NativeMethods.ReleaseDC(process.MainWindowHandle, hdc);
                }

                return new FEZScreenShot(bmp, timestamp);
            }
        }
    }
}
