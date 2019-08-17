using SkillUseCounter.Entity;
using SkillUseCounter.Extension;
using SkillUseCounter.Storage;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace SkillUseCounter.Recognizer
{
    internal class SkillArrayRecognizer : IResettableRecognizer<Skill[]>
    {
        public const Skill[] InvalidSkills = null;

        private const int Threshold = 3;

        // 各スキルが描かれている位置 (x座標のみ画像右側からの相対座標)
        private readonly Rectangle[] SkillRectTable = new Rectangle[]
        {
            new Rectangle(-30,  22, 12, 12),
            new Rectangle(-30,  54, 12, 12),
            new Rectangle(-30,  86, 12, 12),
            new Rectangle(-30, 118, 12, 12),
            new Rectangle(-30, 150, 12, 12),
            new Rectangle(-30, 182, 12, 12),
            new Rectangle(-30, 214, 12, 12),
            new Rectangle(-30, 246, 12, 12),
        };

        private object _obj = new object();

        private Skill[] _previousSkill = InvalidSkills;

        public event EventHandler<Skill[]> Updated;

        public Skill[] Recognize(Bitmap bitmap)
        {
            // スキル取得
            var skills = GetSkills(bitmap);
            if (skills != InvalidSkills)
            {
                // 前回から更新されていれば通知
                if (_previousSkill == InvalidSkills || !_previousSkill.SequenceEqual(skills, x => x.Name))
                {
                    _previousSkill = skills;
                    Updated?.Invoke(this, skills);
                }
            }
            else
            {
                // 失敗していれば前回の値をそのまま使う
                skills = _previousSkill;
            }

            return skills;
        }

        public void Reset()
        {
            _previousSkill = InvalidSkills;
            Updated?.Invoke(this, InvalidSkills);
        }

        private Skill[] GetSkills(Bitmap bitmap)
        {
            var skills = new Skill[SkillRectTable.Length];

            Parallel.For(0, SkillRectTable.Length, i =>
            {
                Bitmap b;
                lock (_obj)
                {
                    var r = SkillRectTable[i];
                    r.X = bitmap.Width + r.X;
                    b = bitmap.Clone(r, PixelFormat.Format24bppRgb);
                }

                skills[i] = Skill.Empty;

                var barray = b.ConvertToByteArray();

                foreach (var skill in SkillStorage.Table)
                {
                    if (Compare(barray, skill.Value.Data))
                    {
                        skills[i] = skill.Value;
                        break;
                    }
                }

                b.Dispose();
            });

            return skills;
        }

        private bool Compare(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
            {
                return false;
            }

            // 環境によって何故か微妙に色が異なることがあるため、
            // 完全一致ではなく閾値以内であれば一致とする。
            // なおL*a*b色空間では誤認識が多かったため、あえてRGB色空間で色差をみる。
            for (int i = 0; i < b1.Length; i++)
            {
                if (Math.Abs(b1[i] - b2[i]) > Threshold)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
