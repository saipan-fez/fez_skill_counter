using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using SkillUseCounter.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SkillUseCounter.Recognizer
{
    public class KeepDamageRecognizer : IResettableRecognizer<KeepDamage>
    {
        /// <summary>
        /// L*a*b色空間上での色差の閾値
        /// </summary>
        private const double ColorDiffThreashold = 10.0d;

        // 攻撃・防衛側それぞれで走査するy座標
        private const int Y_Attack  = 18;
        private const int Y_Defence = 51;

        private KeepDamage _previousKeepDamage = KeepDamage.Empty;

        private readonly List<Tuple<uint[], int>> AttackKeepDamageMap = new List<Tuple<uint[], int>>()
        {
            new Tuple<uint[], int>(new uint[] { 0xffff852e, 0xffff9f2d }, 3),
            new Tuple<uint[], int>(new uint[] { 0xfff84e35, 0xfffa5d3a }, 2),
            new Tuple<uint[], int>(new uint[] { 0xffcf4143, 0xffd24c47 }, 1),
            new Tuple<uint[], int>(new uint[] { 0xff552205,            }, 0),
        };

        private readonly List<Tuple<uint[], int>> DefenceKeepDamageMap = new List<Tuple<uint[], int>>()
        {
            new Tuple<uint[], int>(new uint[] { 0xff237dfd, 0xff2595fd }, 3),
            new Tuple<uint[], int>(new uint[] { 0xff404df6, 0xff445cf9 }, 2),
            new Tuple<uint[], int>(new uint[] { 0xff6b42d4,            }, 1),
            new Tuple<uint[], int>(new uint[] { 0xff082135,            }, 0),
        };

        public event EventHandler<KeepDamage> Updated;

        public KeepDamage Recognize(Bitmap bitmap)
        {
            var attack  = GetAttackKeepDamage(bitmap);
            var defence = GetDefenceKeepDamage(bitmap);

            var keepDamage = new KeepDamage(attack, defence);

            if (attack  != _previousKeepDamage.AttackKeepDamage ||
                defence != _previousKeepDamage.AttackKeepDamage)
            {
                _previousKeepDamage = keepDamage;
                Updated?.Invoke(this, keepDamage);
            }

            return keepDamage;
        }

        private double GetAttackKeepDamage(Bitmap bitmap)
        {
            return GetKeepDamage(bitmap, Y_Attack, AttackKeepDamageMap);
        }

        private double GetDefenceKeepDamage(Bitmap bitmap)
        {
            return GetKeepDamage(bitmap, Y_Defence, DefenceKeepDamageMap);
        }

        private double GetKeepDamage(Bitmap bitmap, int y, List<Tuple<uint[], int>> damageMap)
        {
            var center = bitmap.Width / 2;
            var leftX  = center - 124;
            var rightX = center + 3;

            var isValid = false;
            var value   = 0;
            var maxX    = rightX;
            for (var x = leftX; x <= rightX; x++)
            {
                var color = (uint)bitmap.GetPixel(x, y).ToArgb();
                int v = -1;
                foreach (var t in damageMap)
                {
                    if (IsSameColorContains(t.Item1, color))
                    {
                        v       = t.Item2;
                        isValid = true;
                        break;
                    }
                }

                if (value < v)
                {
                    value   = v;
                    maxX    = x;
                }
            }

            if (!isValid)
            {
                return KeepDamage.InvalidKeepDamage;
            }

            var vv = Math.Max(0.0, value - 1);
            var ww = (rightX - maxX + 1.0) / (rightX - leftX + 1.0);

            return vv + ww;
        }

        private bool IsSameColorContains(uint[] colors, uint c)
        {
            var difference = new CIE76ColorDifference();
            var converter  = new ColourfulConverter(){ WhitePoint = Illuminants.D65 };
            var labColor   = converter.ToLab(new RGBColor(Color.FromArgb((int)c)));

            foreach (var color in colors)
            {
                var l    = converter.ToLab(new RGBColor(Color.FromArgb((int)color)));
                var diff = difference.ComputeDifference(labColor, l);
                if (Math.Abs(diff) < ColorDiffThreashold)
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            _previousKeepDamage = KeepDamage.Empty;
            Updated?.Invoke(this, _previousKeepDamage);
        }
    }
}
