using FEZSkillCounter.Extension;
using System.Drawing;
using System.Linq;

namespace FEZSkillCounter
{
    public class SkillOcr
    {
        public Skill Process(Bitmap bitmap)
        {
            var hash = bitmap.SHA1Hash();

            return SkillStorage.Table.ContainsKey(hash) ? SkillStorage.Table[hash] : Skill.Empty;
        }
    }
}
