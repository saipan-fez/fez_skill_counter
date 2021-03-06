﻿using SkillUseCounter.Entity;
using System;

namespace SkillUseCounter
{
    public class ProcessTimeUpdatedEventArgs : EventArgs
    {
        public double ProcessAvgTime { get; }
        public ProcessTimeUpdatedEventArgs(double processAvgTime)
        {
            ProcessAvgTime = processAvgTime;
        }
    }

    public class SkillsUpdatedEventArgs : EventArgs
    {
        public Skill[] Skills { get; }
        public SkillsUpdatedEventArgs(Skill[] skills)
        {
            Skills = skills;
        }
    }

    public class PowUpdatedEventArgs : EventArgs
    {
        public int Pow { get; }
        public PowUpdatedEventArgs(int pow)
        {
            Pow = pow;
        }
    }

    public class HpUpdatedEventArgs : EventArgs
    {
        public int Hp { get; }
        public HpUpdatedEventArgs(int hp)
        {
            Hp = hp;
        }
    }

    public class PowDebuffsUpdatedEventArgs : EventArgs
    {
        public PowDebuff[] PowDebuffs { get; }
        public PowDebuffsUpdatedEventArgs(PowDebuff[] powDebuffs)
        {
            PowDebuffs = powDebuffs;
        }
    }

    public class KeepDamageUpdatedEventArgs : EventArgs
    {
        public KeepDamage Damage { get; }
        public KeepDamageUpdatedEventArgs(KeepDamage damage)
        {
            Damage = damage;
        }
    }

    public class BookUsesUpdatedEventArgs : EventArgs
    {
        public bool IsBookUsed { get; }
        public BookUsesUpdatedEventArgs(bool isUsed)
        {
            IsBookUsed = isUsed;
        }
    }

    public class SkillUsedEventArgs : EventArgs
    {
        public Skill UsedSkill { get; }
        public SkillUsedEventArgs(Skill usedSkill)
        {
            UsedSkill = usedSkill;
        }
    }

    public class WarStartedEventArgs : EventArgs
    {
        public Map Map { get; }
        public WarStartedEventArgs(Map map)
        {
            Map = map;
        }
    }

    public class WarFinishedEventArgs : EventArgs
    {
        public Map Map { get; }
        public WarFinishedEventArgs(Map map)
        {
            Map = map;
        }
    }

    public class WarCanceledEventArgs : EventArgs
    {
        public Map Map { get; }
        public WarCanceledEventArgs(Map map)
        {
            Map = map;
        }
    }
}
