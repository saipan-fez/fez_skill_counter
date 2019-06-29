using System.Drawing;
using System.Drawing.Imaging;

namespace PowImageClipper
{
    class Program
    {
        /// <summary>
        /// Powの部分を画像として切り取るプログラム
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            int cnt = 0;
            foreach(var path in args)
            {
                using (var bitmap   = new Bitmap(path))
                {
                    var w = bitmap.Width;
                    var h = bitmap.Height;
                    var f = PixelFormat.Format24bppRgb;

                    using (var houndredsPlace = bitmap.Clone(new Rectangle(w - 143, h - 67, 7, 10), f)) // 百の位
                    using (var tensPlace = bitmap.Clone(new Rectangle(w - 135, h - 67, 7, 10), f))      // 十の位
                    using (var onesPlace = bitmap.Clone(new Rectangle(w - 127, h - 67, 7, 10), f))      // 一の位
                    {
                        houndredsPlace.Save(cnt + "_houndred.bmp");
                        tensPlace.Save(cnt + "_ten.bmp");
                        onesPlace.Save(cnt + "_one.bmp");
                    }
                }

                cnt++;
            }
        }
    }
}
