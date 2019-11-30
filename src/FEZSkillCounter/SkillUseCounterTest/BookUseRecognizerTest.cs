using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillUseCounter.Entity;
using SkillUseCounter.Recognizer;
using System.Drawing;

namespace SkillUseCounterTest
{
    [TestClass]
    public class BookUseRecognizerTest
    {
        BookUseRecognizer recognizer;

        [TestInitialize]
        public void Initialize()
        {
            recognizer = new BookUseRecognizer();
        }

        [TestMethod]
        public void 書の使用有無が正しく取得できるか()
        {
            var ret1 = IsBookUsed("TestImages\\BookUse\\BookOn.png");
            Assert.IsTrue(ret1);

            var ret2 = IsBookUsed("TestImages\\BookUse\\BookOff.png");
            Assert.IsFalse(ret2);
        }

        private bool IsBookUsed(string path)
        {
            using (var bitmap = new Bitmap(path))
            {
                return recognizer.Recognize(bitmap);
            }
        }
    }
}
