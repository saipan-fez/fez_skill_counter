namespace SkillUseCounter.Entity
{
    public class Skill
    {
        private const string UnknownSkillName = "Unknown";

        public static Skill Empty => new Skill(UnknownSkillName, UnknownSkillName, new int[] { int.MaxValue }, false);

        public string Name { get; }
        public string ShortName { get; }
        public int[] Pow { get; }
        public bool IsActive { get; }

        public Skill()
        { }

        public Skill(string name, string shortName, int[] pow, bool isActive)
        {
            Name      = name;
            ShortName = shortName;
            Pow       = pow;
            IsActive  = isActive;
        }

        public static Skill Create(string resourceName, string shortName, params int[] pow)
        {
            var name = resourceName
                .Replace("Cestus_", "")
                .Replace("Fencer_", "")
                .Replace("Scout_", "")
                .Replace("Sorcerer_", "")
                .Replace("Warrior_", "")
                .Replace("_S", "")
                .Replace("_D", "");

            var isActive = (resourceName.IndexOf("_S") != -1);

            return new Skill(name, shortName, pow, isActive);
        }

        public bool IsEmpty()
        {
            return Name == UnknownSkillName;
        }
    }
}
