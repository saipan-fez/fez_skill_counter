using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillUseCounter.Recognizer;
using SkillUseCounter.Storage;
using System.Drawing;

namespace SkillUseCounterTest
{
    [TestClass]
    public class HpRecognizerTest
    {
        HpRecognizer recognizer;

        [TestInitialize]
        public void Initialize()
        {
            PowStorage.Create();
            recognizer = new HpRecognizer();
        }

        [TestMethod]
        public void Pow100()
        {
            using (var b = new Bitmap("TestImages\\pow\\100.png"))
            {
                var hp = recognizer.Recognize(b);
                Assert.AreEqual(1000, hp);
            }
        }
    }
}
