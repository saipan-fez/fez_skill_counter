using FEZSkillUseCounter.Common;
using FEZSkillUseCounter.Entity;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace FEZSkillUseCounter
{
    public class FEZScreenShotStorage
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
