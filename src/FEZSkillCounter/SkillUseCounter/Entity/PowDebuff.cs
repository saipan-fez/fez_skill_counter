using System;

namespace SkillUseCounter.Entity
{
    public class PowDebuff
    {
        private static int idSource = 0;

        public int Id { get; }
        public string Name { get; }
        public int[] Pow { get; }
        public int EffectCount { get; }
        public TimeSpan EffectTimeSpan { get; }

        public PowDebuff(string name, int[] pow, int effectCount, TimeSpan effectTimeSpan)
        {
            Id = idSource++;
            Name = name;
            Pow = pow;
            EffectCount = effectCount;
            EffectTimeSpan = effectTimeSpan;
        }
    }
}
