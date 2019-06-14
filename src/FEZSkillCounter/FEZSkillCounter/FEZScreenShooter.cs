using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace FEZSkillCounter
{
    public class FEZScreenShooter
    {
        public Bitmap Shoot()
        {
            using (var p = Process.GetProcessesByName("FEzero_Client").FirstOrDefault())
            {
                if (p == null)
                {
                    return null;
                }

                if (!NativeMethods.GetWindowRect(p.MainWindowHandle, out RECT rect))
                {
                    return null;
                }

                Size size  = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                Bitmap bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, size, CopyPixelOperation.SourceCopy);
                }

                return bmp;
            }
        }
    }
}
