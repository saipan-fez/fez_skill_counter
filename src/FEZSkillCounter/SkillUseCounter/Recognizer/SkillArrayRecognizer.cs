using SkillUseCounter.Entity;
using SkillUseCounter.Extension;
using SkillUseCounter.Storage;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SkillUseCounter.Recognizer
{
    internal class SkillArrayRecognizer : IResettableRecognizer<Skill[]>
    {
        public const Skill[] InvalidSkills = null;

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
        }

        private Skill[] GetSkills(Bitmap bitmap)
        {
            Skill[] skills = new Skill[SkillRectTable.Length];

            for (int i = 0; i < SkillRectTable.Length; i++)
            {
                var r = SkillRectTable[i];
                r.X = bitmap.Width + r.X;

                using (var b = bitmap.Clone(r, PixelFormat.Format24bppRgb))
                {
                    var hash = b.SHA1Hash();

                    skills[i] = SkillStorage.Table.ContainsKey(hash) ?
                        SkillStorage.Table[hash] :
                        Skill.Empty;
                }
            }

            return skills;
        }
    }
}
