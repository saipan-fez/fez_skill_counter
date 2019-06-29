using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillUseCounter.Recognizer;
using SkillUseCounter.Storage;
using System.Drawing;

namespace SkillUseCounterTest
{
    [TestClass]
    public class PowRecognizerTest
    {
        PowRecognizer recognizer;

        [TestInitialize]
        public void Initialize()
        {
            PowStorage.Create();
            recognizer = new PowRecognizer();
        }

        [TestMethod]
        public void Pow100()
        {
            using (var b = new Bitmap("TestImages\\pow\\100.png"))
            {
                var pow = recognizer.Recognize(b);
                Assert.AreEqual(100, pow);
            }
        }

        [TestMethod]
        public void Pow32()
        {
            using (var b = new Bitmap("TestImages\\pow\\32.png"))
            {
                var pow = recognizer.Recognize(b);
                Assert.AreEqual(32, pow);
            }
        }

        [TestMethod]
        public void Pow56()
        {
            using (var b = new Bitmap("TestImages\\pow\\56.png"))
            {
                var pow = recognizer.Recognize(b);
                Assert.AreEqual(56, pow);
            }
        }

        [TestMethod]
        public void Pow72()
        {
            using (var b = new Bitmap("TestImages\\pow\\72.png"))
            {
                var pow = recognizer.Recognize(b);
                Assert.AreEqual(72, pow);
            }
        }

        [TestMethod]
        public void Pow84()
        {
            using (var b = new Bitmap("TestImages\\pow\\84.png"))
            {
                var pow = recognizer.Recognize(b);
                Assert.AreEqual(84, pow);
            }
        }

        [TestMethod]
        public void Pow88()
        {
            using (var b = new Bitmap("TestImages\\pow\\88.png"))
            {
                var pow = recognizer.Recognize(b);
                Assert.AreEqual(88, pow);
            }
        }

        [TestMethod]
        public void Pow90()
        {
            using (var b = new Bitmap("TestImages\\pow\\90.png"))
            {
                var pow = recognizer.Recognize(b);
                Assert.AreEqual(90, pow);
            }
        }
    }
}
