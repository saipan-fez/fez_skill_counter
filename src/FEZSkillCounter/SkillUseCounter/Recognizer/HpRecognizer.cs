using SkillUseCounter.Extension;
using SkillUseCounter.Storage;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SkillUseCounter.Recognizer
{
    internal class PowRecognizer : IResettableRecognizer<int>
    {
        public const int InvalidPow = int.MaxValue;

        private int _previousPow = InvalidPow;

        public event EventHandler<int> Updated;

        public int Recognize(Bitmap bitmap)
        {
            // Powを取得
            var pow = GetPow(bitmap);
            if (pow != int.MaxValue)
            {
                // 前回から更新されていれば通知
                if (_previousPow == InvalidPow || _previousPow != pow)
                {
                    _previousPow = pow;
                    Updated?.Invoke(this, pow);
                }
            }
            else
            {
                // 失敗していれば前回の値をそのまま使う
                pow = _previousPow;
            }

            return pow;
        }

        public void Reset()
        {
            _previousPow = InvalidPow;
            Updated?.Invoke(this, _previousPow);
        }

        private int GetPow(Bitmap bitmap)
        {
            var pow = int.MaxValue;

            var w = bitmap.Width;
            var h = bitmap.Height;
            var f = PixelFormat.Format24bppRgb;

            using (var houndredsPlace = bitmap.Clone(new Rectangle(w - 143, h - 67, 7, 10), f))     // 百の位
            using (var tensPlace      = bitmap.Clone(new Rectangle(w - 135, h - 67, 7, 10), f))     // 十の位
            using (var onesPlace      = bitmap.Clone(new Rectangle(w - 127, h - 67, 7, 10), f))     // 一の位
            {
                var houndreds = GetPowNum(houndredsPlace);
                var tens      = GetPowNum(tensPlace);
                var ones      = GetPowNum(onesPlace);

                if (houndreds != int.MaxValue &&
                    tens != int.MaxValue &&
                    ones != int.MaxValue)
                {
                    pow = houndreds * 100 + tens * 10 + ones;
                }
                else
                {
                    Logger.WriteLine($"Pow detect error. houndred:{houndreds} tens:{tens} ones:{ones}");
                }
            }

            return pow;
        }

        private int GetPowNum(Bitmap bitmap)
        {
            var hash = bitmap.FillPaddingToZero().SHA1Hash();
            if (PowStorage.Table.TryGetValue(hash, out int pow))
            {
                return pow;
            }

            // 上記に当てはまらないものはエラーとする
            return int.MaxValue;
        }
    }
}
