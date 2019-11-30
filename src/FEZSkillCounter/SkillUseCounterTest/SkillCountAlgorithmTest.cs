using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillUseCounter.Algorithm;
using SkillUseCounter.Entity;

namespace SkillUseCounterTest
{
    [TestClass]
    public class SkillCountAlgorithmTest
    {
        SkillUseAlgorithm algo;

        private readonly Skill[] TestSkill = new Skill[]
        {
            new Skill("Name", "ShorName", "workName", new int[]{ 10, 15, 20 }, true)
        };
        private readonly PowDebuff[] EmptyPowDebuff = new PowDebuff[0];
        private readonly PowDebuff[] PowerBreak = new PowDebuff[]
        {
            new PowDebuff("パワーブレイク", new int[]{ -15, -20, -25 }, 8, TimeSpan.FromSeconds(3))
        };

        [TestInitialize]
        public void Initialize()
        {
            algo = new SkillUseAlgorithm();
        }

        [TestMethod]
        public void Pow減少なし()
        {
            Assert.IsNull(algo.RecognizeUsedSkill(0, 100, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(20, 100, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(40, 100, TestSkill, EmptyPowDebuff));
        }

        [TestMethod]
        public void Pow回復()
        {
            Assert.IsNull(algo.RecognizeUsedSkill(0, 90, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(20, 90, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(40, 93, TestSkill, EmptyPowDebuff));
        }

        [TestMethod]
        public void スキル使用によるPow減少()
        {
            Assert.IsNull(algo.RecognizeUsedSkill(20, 90, TestSkill, EmptyPowDebuff));
            Assert.IsNotNull(algo.RecognizeUsedSkill(40, 70, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(60, 70, TestSkill, EmptyPowDebuff));
        }

        [TestMethod]
        public void パワブレ()
        {
            Assert.IsNull(algo.RecognizeUsedSkill(30, 90, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(50, 90, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(70, 90, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(90, 90, TestSkill, PowerBreak));

            // これは最初のフレーム(TimeStamp=30)からぴったり3秒後なので、スキル使用
            Assert.IsNotNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 30, 70, TestSkill, PowerBreak));
            Assert.AreEqual(algo.DebuffList.Count, 8);

            // これはそこから時間経過しているため、パワブレ
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 31, 50, TestSkill, PowerBreak));
            Assert.AreEqual(algo.DebuffList.Count, 7);
        }

        [TestMethod]
        public void パワブレ_ラグによってPow減少がずれたケース()
        {
            Assert.IsNull(algo.RecognizeUsedSkill(30, 90, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(50, 90, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(70, 90, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(90, 90, TestSkill, PowerBreak));

            // 本来パワブレでPowが減少するタイミングだが、
            // 何らかの理由でPow減少タイミングがずれたことが考慮できているかチェックする
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 31, 90, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 51, 50, TestSkill, PowerBreak));
        }

        [TestMethod]
        public void パワブレ中にデッド()
        {
            Assert.IsNull(algo.RecognizeUsedSkill(30, 90, TestSkill, EmptyPowDebuff));
            Assert.IsNull(algo.RecognizeUsedSkill(50, 90, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(70, 90, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(90, 90, TestSkill, PowerBreak));

            // パワブレくらう
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 31, 70, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 51, 70, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 71, 70, TestSkill, PowerBreak));
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 91, 70, TestSkill, PowerBreak));

            // デッド
            Assert.IsNull(algo.RecognizeUsedSkill(TimeSpan.TicksPerSecond * 3 + 101, 70, TestSkill, EmptyPowDebuff));
            Assert.AreEqual(algo.DebuffList.Count, 0);
        }
    }
}
