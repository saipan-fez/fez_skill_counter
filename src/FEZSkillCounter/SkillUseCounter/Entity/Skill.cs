namespace FEZSkillUseCounter.Entity
{
    public class Skill
    {
        private const string UnknownSkillName = "Unknown";

        public string Name { get; }
        public int[] Pow { get; }
        public bool IsActive { get; }

        public Skill()
        { }

        public Skill(string name, int[] pow, bool isActive)
        {
            Name = name;
            Pow = pow;
            IsActive = isActive;
        }

        public static Skill CreateFromResourceFileName(string resourceName, params int[] pow)
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

            return new Skill(name, pow, isActive);
        }

        public static Skill Empty
        {
            get
            {
                return new Skill("Unknown", new int[] { int.MaxValue }, false);
            }
        }

        public bool IsEmpty()
        {
            return Name == UnknownSkillName;
        }
    }
}
