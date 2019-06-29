using System;
using System.Drawing;

namespace SkillUseCounter.Entity
{
    internal class FEZScreenShot : IDisposable
    {
        public Bitmap Image { get; }
        public long TimeStamp { get; }

        public FEZScreenShot(Bitmap image, long timeStamp)
        {
            Image = image;
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
}
