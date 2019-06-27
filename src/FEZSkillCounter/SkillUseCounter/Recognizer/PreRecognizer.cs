using System.Drawing;

namespace FEZSkillUseCounter.Recognizer
{
    /// <summary>
    /// 他の解析前に解析可能かチェックするクラス
    /// </summary>
    public class PreRecognizer : IRecognizer<bool>
    {
        public bool Recognize(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return false;
            }

            var w = bitmap.Width;
            var h = bitmap.Height;

            bool ret = true;

            // 画面右上のスキル枠があるかどうか
            ret &= bitmap.GetPixel(w - 47, 10) == Color.FromArgb(178, 186, 192);
            ret &= bitmap.GetPixel(w - 47, 11) == Color.FromArgb(168, 106, 121);

            // 画面右下にHP/Pow枠があるかどうか
            ret &= bitmap.GetPixel(w - 315, h - 54) == Color.FromArgb(168, 160, 158);
            ret &= bitmap.GetPixel(w - 314, h - 54) == Color.FromArgb( 29,  27,  26);

            return ret;
        }
    }
}
