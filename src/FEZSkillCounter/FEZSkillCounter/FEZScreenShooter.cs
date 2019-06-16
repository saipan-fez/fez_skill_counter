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

                Size size  = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                Bitmap bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, size, CopyPixelOperation.SourceCopy);
                }
                
                return new FEZScreenShot(bmp, tick);
            }
        }
    }
}
