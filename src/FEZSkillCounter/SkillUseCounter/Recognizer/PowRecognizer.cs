﻿using SkillUseCounter.Extension;
using SkillUseCounter.Storage;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SkillUseCounter.Recognizer
{
    internal class HpRecognizer : IResettableRecognizer<int>
    {
        public const int InvalidHp = int.MaxValue;

        private int _previousPow = InvalidHp;

        public event EventHandler<int> Updated;

        public int Recognize(Bitmap bitmap)
        {
            // Powを取得
            var hp = GetHp(bitmap);
            if (hp != int.MaxValue)
            {
                // 前回から更新されていれば通知
                if (_previousPow == InvalidHp || _previousPow != hp)
                {
                    _previousPow = hp;
                    Updated?.Invoke(this, hp);
                }
            }
            else
            {
                // 失敗していれば前回の値をそのまま使う
                hp = _previousPow;
            }

            return hp;
        }

        public void Reset()
        {
            _previousPow = InvalidHp;
            Updated?.Invoke(this, _previousPow);
        }

        private int GetHp(Bitmap bitmap)
        {
            var pow = int.MaxValue;

            var w = bitmap.Width;
            var h = bitmap.Height;
            var f = PixelFormat.Format24bppRgb;

            using (var thousandsPlace = bitmap.Clone(new Rectangle(w - 151, h - 82, 7, 10), f))     // 千の位
            using (var houndredsPlace = bitmap.Clone(new Rectangle(w - 143, h - 82, 7, 10), f))     // 百の位
            using (var tensPlace      = bitmap.Clone(new Rectangle(w - 135, h - 82, 7, 10), f))     // 十の位
            using (var onesPlace      = bitmap.Clone(new Rectangle(w - 127, h - 82, 7, 10), f))     // 一の位
            {
                var thousands = GetPowNum(thousandsPlace);
                var houndreds = GetPowNum(houndredsPlace);
                var tens      = GetPowNum(tensPlace);
                var ones      = GetPowNum(onesPlace);

                if (thousands != int.MaxValue &&
                    houndreds != int.MaxValue &&
                    tens != int.MaxValue &&
                    ones != int.MaxValue)
                {
                    pow = thousands * 1000 + houndreds * 100 + tens * 10 + ones;
                }
                else
                {
                    Logger.WriteLine($"Hp detect error. houndred:{houndreds} tens:{tens} ones:{ones}");
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
