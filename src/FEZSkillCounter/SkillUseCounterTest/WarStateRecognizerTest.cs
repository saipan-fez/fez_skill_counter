using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SkillUseCounter.Entity;
using SkillUseCounter.Recognizer;
using SkillUseCounter.Storage;
using System.Drawing;

namespace SkillUseCounterTest
{
    [TestClass]
    public class WarStateRecognizerTest
    {
        WarStateRecognizer recognizer;

        [TestInitialize]
        public void Initialize()
        {
            MapStorage.Create();
            recognizer = new WarStateRecognizer(new MapRecognizer());
        }

        [TestMethod]
        public void 戦争開始したか()
        {
            var str = "";
            recognizer.WarStarted += (s, e) => str = "WarStarted";

            using (var startBitmap = new Bitmap("TestImages\\WarStarted.png"))
            {
                recognizer.Report(startBitmap);

                // イベントが発火したかどうかチェック
                Assert.AreEqual("WarStarted", str);
            }
        }

        [TestMethod]
        public void 戦争終了したか()
        {
            var str = "";
            recognizer.WarFinished += (s, e) => str = "WarFinished";

            using (var startBitmap = new Bitmap("TestImages\\WarStarted.png"))
            using (var endBitmap = new Bitmap("TestImages\\WarFinished.png"))
            {
                recognizer.Report(startBitmap);
                recognizer.Report(endBitmap);

                // イベントが発火したかどうかチェック
                Assert.AreEqual("WarFinished", str);
            }
        }
    }
}
