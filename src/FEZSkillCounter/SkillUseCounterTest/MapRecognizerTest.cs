using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillUseCounter.Recognizer;
using SkillUseCounter.Storage;
using System.Drawing;

namespace SkillUseCounterTest
{
    [TestClass]
    public class MapRecognizerTest
    {
        MapRecognizer recognizer;

        [TestInitialize]
        public void Initialize()
        {
            MapStorage.Create();
            recognizer = new MapRecognizer();
        }

        [TestMethod]
        public void マップ名が取得できているか()
        {
            using (var bitmap = new Bitmap("TestImages\\MapTest.png"))
            {
                var map = recognizer.Recognize(bitmap);
                Assert.AreEqual(map.Name, "ダガー島");
            }
        }
    }
}
