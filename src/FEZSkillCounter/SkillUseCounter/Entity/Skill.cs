using System.Drawing;
using SkillUseCounter.Extension;

namespace SkillUseCounter.Entity
{
    public class Skill
    {
        private const string UnknownSkillName = "Unknown";
        private const string UnknownWorkName = "Unknown";

        public static Skill Empty => new Skill(UnknownSkillName, UnknownSkillName, UnknownWorkName, new int[] { int.MaxValue }, false);

        public string Name      { get; }
        public string ShortName { get; }
        public string WorkName  { get; }
        public int[]  Pow       { get; }
        public bool   IsActive  { get; }
        public byte[] Data      { get; }

        public Skill()
        { }

        public Skill(string name, string shortName, string workName, int[] pow, bool isActive, byte[] data = null)
        {
            Name      = name;
            ShortName = shortName;
            WorkName  = workName;
            Pow       = pow;
            IsActive  = isActive;
            Data      = data;
        }

        public static Skill CreateFromResource(string resourceName, Bitmap bitmap, string shortName, params int[] pow)
        {
            var name = resourceName
                .Replace("Cestus_", "")
                .Replace("Fencer_", "")
                .Replace("Scout_", "")
                .Replace("Sorcerer_", "")
                .Replace("Warrior_", "")
                .Replace("_S", "")
                .Replace("_D", "");

            var workName = UnknownWorkName;
            {
                var idx = resourceName.IndexOf("_");
                if (idx != -1)
                {
                    var w = resourceName.Substring(0, idx + 1);
                    switch (w)
                    {
                        case "Cestus":
                            workName = "セスタス";
                            break;
                        case "Fencer":
                            workName = "フェンサー";
                            break;
                        case "Scout":
                            workName = "スカウト";
                            break;
                        case "Sorcerer":
                            workName = "ソーサラー";
                            break;
                        case "Warrior":
                            workName = "ウォーリアー";
                            break;
                    }
                }
            }

            var isActive = (resourceName.IndexOf("_S") != -1);

            return new Skill(name, shortName, workName, pow, isActive, bitmap.ConvertToByteArray());
        }

        public bool IsEmpty()
        {
            return Name == UnknownSkillName;
        }
    }
}
