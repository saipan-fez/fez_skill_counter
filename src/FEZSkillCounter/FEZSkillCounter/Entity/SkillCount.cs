using FEZSkillUseCounter.Entity;

namespace FEZSkillCounter.Entity
{
    public class SkillCount
    {
        public Skill Skill { get; }
        public int Count { get; private set; }

        public SkillCount(Skill skill)
        {
            Skill = skill;
            Count = 0;
        }

        public void Increment()
        {
            Count++;
        }
    }
}
