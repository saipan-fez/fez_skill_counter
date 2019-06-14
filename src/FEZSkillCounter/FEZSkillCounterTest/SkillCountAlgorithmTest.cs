using System;
using FEZSkillCounter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FEZSkillCounterTest
{
    [TestClass]
    public class SkillCountAlgorithmTest
    {
        SkillCountAlgorithm algo;

        private readonly PowDebuff[] EmptyPowDebuff = new PowDebuff[0];
        private readonly Skill TestSkill = new Skill("Test", new int[]{ 10, 15, 20 }, true);
        

        [TestInitialize]
        public void Initialize()
        {
            algo = new SkillCountAlgorithm();
        }

        [TestMethod]
        public void Pow減少なし()
        {
            Assert.IsFalse(algo.IsSkillUsed(0, 100, TestSkill, EmptyPowDebuff));
            Assert.IsFalse(algo.IsSkillUsed(0, 100, TestSkill, EmptyPowDebuff));
            Assert.IsFalse(algo.IsSkillUsed(0, 100, TestSkill, EmptyPowDebuff));
        }

        [TestMethod]
        public void Pow回復()
        {
            Assert.IsFalse(algo.IsSkillUsed(0, 90, TestSkill, EmptyPowDebuff));
            Assert.IsFalse(algo.IsSkillUsed(0, 90, TestSkill, EmptyPowDebuff));
            Assert.IsFalse(algo.IsSkillUsed(0, 93, TestSkill, EmptyPowDebuff));
        }

        [TestMethod]
        public void スキル使用()
        {
            Assert.IsFalse(algo.IsSkillUsed(0, 90, TestSkill, EmptyPowDebuff));
            Assert.IsTrue(algo.IsSkillUsed(0, 70, TestSkill, EmptyPowDebuff));
            Assert.IsFalse(algo.IsSkillUsed(0, 70, TestSkill, EmptyPowDebuff));
        }
    }
}
