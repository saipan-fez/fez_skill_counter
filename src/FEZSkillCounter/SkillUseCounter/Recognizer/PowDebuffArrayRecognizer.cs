using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using SkillUseCounter.Extension;
using SkillUseCounter.Entity;
using System;
using System.Drawing;
using System.Linq;

namespace SkillUseCounter.Recognizer
{
    internal class PowDebuffArrayRecognizer : IResettableRecognizer<PowDebuff[]>
    {
        public const PowDebuff[] InvalidPowDebuffs = null;

        /// <summary>
        /// 右下アイコンの最大個数
        /// </summary>
        /// <remarks>
        /// 最大個数は未調査。
        /// この数までバフが増えないだろうという想定での個数。
        /// </remarks>
        private const int MaxPowerDebuffCount = 10;
        private const double ColorDiffThreashold = 10.0d;

        // パワーブレイクかどうか判定する色
        private readonly RGBColor PowerBreakCmpColor1 = new RGBColor (Color.FromArgb(30, 125, 183));
        private readonly RGBColor PowerBreakCmpColor2 = new RGBColor (Color.FromArgb(49, 134, 187));

        // パワーブレイク
        //   Lv1-3で減少Powが異なる
        private readonly PowDebuff PowerBreak =
            new PowDebuff("パワーブレイク", new int[]{ -15, -20, -25 }, 8, TimeSpan.FromSeconds(3));

        private PowDebuff[] _previousPowDebuffs = InvalidPowDebuffs;

        public event EventHandler<PowDebuff[]> Updated;

        public PowDebuff[] Recognize(Bitmap bitmap)
        {
            // デバフ取得
            var powDebuffs = GetPowDebuffs(bitmap);
            if (powDebuffs != null)
            {
                // 前回から更新されていれば通知
                if (_previousPowDebuffs == null || !_previousPowDebuffs.SequenceEqual(powDebuffs, x => x.Name))
                {
                    _previousPowDebuffs = powDebuffs;
                    Updated?.Invoke(this, powDebuffs);
                }
            }
            else
            {
                // 失敗していれば前回の値をそのまま使う
                powDebuffs = _previousPowDebuffs;
            }

            return powDebuffs;
        }

        public void Reset()
        {
            _previousPowDebuffs = InvalidPowDebuffs;
            Updated?.Invoke(this, _previousPowDebuffs);
        }

        private PowDebuff[] GetPowDebuffs(Bitmap bitmap)
        {
            // 右下のアイコンは透過が少しあるため、ピクセル単位の色が完全一致しない。
            // そのため、L*a*b色空間での色差が閾値以内かどうかで判定する。
            var converter = new ColourfulConverter(){ WhitePoint = Illuminants.D65 };
            var powerBreakCmpLabColor1 = converter.ToLab(PowerBreakCmpColor1);
            var powerBreakCmpLabColor2 = converter.ToLab(PowerBreakCmpColor2);
            var difference = new CIE76ColorDifference();

            for (int i = 0; i < MaxPowerDebuffCount; i++)
            {
                var x1 = bitmap.Width  - 28 - (32 * i);
                var x2 = bitmap.Width  - 34 - (32 * i);
                var y  = bitmap.Height - 20;

                var c1 = converter.ToLab(new RGBColor(bitmap.GetPixel(x1, y)));
                var c2 = converter.ToLab(new RGBColor(bitmap.GetPixel(x2, y)));

                var diff1 = difference.ComputeDifference(powerBreakCmpLabColor1, c1);
                var diff2 = difference.ComputeDifference(powerBreakCmpLabColor2, c2);

                if (Math.Abs(diff1) < ColorDiffThreashold &&
                    Math.Abs(diff2) < ColorDiffThreashold)
                {
                    return new PowDebuff[]
                    {
                        PowerBreak
                    };
                }
            }

            return new PowDebuff[0];
        }
    }
}
