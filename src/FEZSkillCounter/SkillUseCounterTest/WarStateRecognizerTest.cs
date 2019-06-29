using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SkillUseCounter.Entity;
using SkillUseCounter.Recognizer;
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
            var mock = Substitute.For<IRecognizer<Map>>();
            {
                mock.Recognize(Arg.Any<Bitmap>()).Returns(new Map("test"));
            }

            recognizer = new WarStateRecognizer(mock);
        }

        [TestMethod]
        public void 戦争開始したか()
        {
            //using (var startBitmap = new Bitmap("TestImages\\WarFinished.png"))
            //{
            //    var state = recognizer.Recognize(startBitmap);
            //    Assert.AreEqual(state, WarState.AtWar);
            //}
        }

        [TestMethod]
        public void 戦争終了したか()
        {
            //using (var startBitmap = new Bitmap("TestImages\\WarFinished.png"))
            //using (var endBitmap   = new Bitmap("TestImages\\WarFinished.png"))
            //{
            //    var state1 = recognizer.Recognize(startBitmap);
            //    Assert.AreEqual(state1, WarState.AtWar);

            //    var state2 = recognizer.Recognize(startBitmap);
            //    Assert.AreEqual(state2, WarState.Waiting);
            //}
        }
    }
}
