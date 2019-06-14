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

            return SkillDictionary.List.ContainsKey(hash) ? SkillDictionary.List[hash] : null;
        }
    }
}
