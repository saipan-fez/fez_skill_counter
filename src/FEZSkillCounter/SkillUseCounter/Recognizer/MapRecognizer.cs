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
                // 二値化
                b.ToThresholding(true);
                b.FillPaddingToZero();

                // ハッシュ値で該当するマップ名があるかチェック
                var hash = b.SHA1Hash();
                if (MapStorage.Table.TryGetValue(hash, out Map map))
                {
                    return map;
                }

                /*
                 * 闘技場マップ名と一致するかチェックする
                 * 
                 * 闘技場のマップは 「闘技場/闘技場01」 のように部屋番号がマップ名に含まれる。
                 * 画像一致でのマップ名解析では部屋番号をすべて用意する必要があるが、
                 * 現実的にすべてを用意することは難しいため、
                 * 部屋番号の部分(x=78以降)を白で塗りつぶしてチェックする
                 */
                // x=78以降を白で塗りつぶし
                for (var y = 0; y < b.Height; y++)
                {
                    for (var x = 78; x < b.Width; x++)
                    {
                        b.SetPixel(x, y, Color.White);
                    }
                }

                // 再度ハッシュ値で該当するマップ名があるかチェック
                var reHash = b.SHA1Hash();
                if (MapStorage.Table.TryGetValue(reHash, out Map reMap))
                {
                    return reMap;
                }

                return Map.Empty;
            }
        }
    }
}
