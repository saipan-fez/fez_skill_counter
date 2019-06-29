using SkillUseCounter.Entity;
using SkillUseCounter.Extension;
using SkillUseCounter.Storage;
using System.Drawing;
using System.Drawing.Imaging;

namespace SkillUseCounter.Recognizer
{
    internal class MapRecognizer : IRecognizer<Map>
    {
        private readonly Rectangle MapNameRectangle = new Rectangle(103, 10, 160, 11);

        public Map Recognize(Bitmap bitmap)
        {
            using (var b = bitmap.Clone(MapNameRectangle, PixelFormat.Format24bppRgb))
            {
                b.ToThresholding(true);

                var hash = b.FillPaddingToZero().SHA1Hash();

                return MapStorage.Table.TryGetValue(hash, out Map map) ? map : Map.Empty;
            }
        }
    }
}
