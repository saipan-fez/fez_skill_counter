using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using System;
using System.Drawing;

namespace SkillUseCounter.Recognizer
{
    public class BookUseRecognizer : IResettableRecognizer<bool>
    {
        /// <summary>
        /// L*a*b色空間上での色差の閾値
        /// </summary>
        /// <remarks>
        /// 若干透過が入っているためか色差を大きめにとる
        /// (10.0dでは一部超える場合があった)
        /// </remarks>
        private const double ColorDiffThreashold = 15.0d;

        private bool _previousUsed = false;

        public event EventHandler<bool> Updated;

        public bool Recognize(Bitmap bitmap)
        {
            var isUsed = IsBookUsed(bitmap);
            if (isUsed != _previousUsed)
            {
                _previousUsed = isUsed;
                Updated?.Invoke(this, isUsed);
            }

            return isUsed;
        }

        public void Reset()
        {
            _previousUsed = false;
            Updated?.Invoke(this, _previousUsed);
        }

        private bool IsBookUsed(Bitmap bitmap)
        {
            bool ret = true;

            ret &= Compare(bitmap, 432, 27, Color.FromArgb(255, 135,  15));
            ret &= Compare(bitmap, 431, 27, Color.FromArgb(255, 255, 214));
            ret &= Compare(bitmap, 449, 10, Color.FromArgb(255, 255,   3));

            return ret;
        }

        private bool Compare(Bitmap bitmap, int x, int y, Color color)
        {
            var difference = new CIE76ColorDifference();
            var converter  = new ColourfulConverter(){ WhitePoint = Illuminants.D65 };
            var rgbColor   = bitmap.GetPixel(bitmap.Width - x, bitmap.Height - y);
            var labColor   = converter.ToLab(new RGBColor(rgbColor));
            var cmpColor   = converter.ToLab(new RGBColor(color));
            var diff       = difference.ComputeDifference(labColor, cmpColor);

            return Math.Abs(diff) < ColorDiffThreashold;
        }
    }
}
