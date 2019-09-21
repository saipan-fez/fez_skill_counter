using FEZSkillCounter.Model.Entity;

namespace FEZSkillCounter.Model.Repository
{
    public static class SkillCountDetailEntityExtension
    {
        public static void Increment(this SkillCountDetailEntity own)
        {
            own.Count++;
        }

        public static void Reset(this SkillCountDetailEntity own)
        {
            own.Count = 0;
        }
    }
}
