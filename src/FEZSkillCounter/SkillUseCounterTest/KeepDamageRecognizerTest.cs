using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillUseCounter.Entity;
using SkillUseCounter.Recognizer;
using System.Drawing;

namespace SkillUseCounterTest
{
    [TestClass]
    public class KeepDamageRecognizerTest
    {
        KeepDamageRecognizer recognizer;

        [TestInitialize]
        public void Initialize()
        {
            recognizer = new KeepDamageRecognizer();
        }

        [TestMethod]
        public void 領域ダメージが正しく取得できているか()
        {
            var kd1 = GetKeepDamage("TestImages\\KeepDamage\\A0.0-D0.9.png");
            Assert.AreEqual(0.0, kd1.AttackKeepDamage, 0.05);
            Assert.AreEqual(0.9, kd1.DefenceKeepDamage, 0.05);

            var kd2 = GetKeepDamage("TestImages\\KeepDamage\\A1.6-D1.0.png");
            Assert.AreEqual(1.6, kd2.AttackKeepDamage, 0.05);
            Assert.AreEqual(1.0, kd2.DefenceKeepDamage, 0.05);

            var kd3 = GetKeepDamage("TestImages\\KeepDamage\\A2.0-D1.7.png");
            Assert.AreEqual(2.0, kd3.AttackKeepDamage, 0.05);
            Assert.AreEqual(1.7, kd3.DefenceKeepDamage, 0.05);

            var kd4 = GetKeepDamage("TestImages\\KeepDamage\\A2.3-D2.0.png");
            Assert.AreEqual(2.3, kd4.AttackKeepDamage, 0.05);
            Assert.AreEqual(2.0, kd4.DefenceKeepDamage, 0.05);

            var kd5 = GetKeepDamage("TestImages\\KeepDamage\\A3.0-D3.0.png");
            Assert.AreEqual(3.0, kd5.AttackKeepDamage);
            Assert.AreEqual(3.0, kd5.DefenceKeepDamage);

            var kd6 = GetKeepDamage("TestImages\\KeepDamage\\Invalid.png");
            Assert.AreEqual(KeepDamage.InvalidKeepDamage, kd6.AttackKeepDamage);
            Assert.AreEqual(KeepDamage.InvalidKeepDamage, kd6.DefenceKeepDamage);
        }

        private KeepDamage GetKeepDamage(string path)
        {
            using (var bitmap = new Bitmap(path))
            {
                return recognizer.Recognize(bitmap);
            }
        }
    }
}
