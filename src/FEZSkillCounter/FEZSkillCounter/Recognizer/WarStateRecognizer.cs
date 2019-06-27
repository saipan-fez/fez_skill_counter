using System;
using System.Drawing;

namespace FEZSkillCounter.Recognizer
{
    public enum WarState
    {
        /// <summary>
        /// 戦争中
        /// </summary>
        AtWar,
        /// <summary>
        /// 待機中(戦争中ではない)
        /// </summary>
        Waiting,
    }

    public class WarStateRecognizer : IRecognizer<WarState>
    {
        /// <summary>
        /// 現在の状態
        /// </summary>
        public WarState State { get; private set; } = WarState.Waiting;

        public WarState Recognize(Bitmap bitmap)
        {
            switch (State)
            {
                // 戦争中なら、戦争が終了したかどうかチェックする
                case WarState.AtWar:
                    if (IsWarFinished(bitmap))
                    {
                        State = WarState.Waiting;
                    }
                    break;

                // 戦争待機中なら、戦争が開始したかどうかチェックする
                case WarState.Waiting:
                    if (IsWarStarted(bitmap))
                    {
                        State = WarState.AtWar;
                    }
                    break;

                default:
                    throw new Exception("invalid state");
            }

            return State;
        }

        private bool IsWarStarted(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return false;
            }

            bool ret = true;

            var w = bitmap.Width;
            var h = bitmap.Height;

            var center = new Point(bitmap.Size.Width / 2, bitmap.Size.Height / 2);

            // TODO: 確認する座標と色
            // 「戦闘開始」が画面に表示されているかどうかで判定する
            // ※途中参戦であっても参戦直後に表示される
            ret &= bitmap.GetPixel(center.X - 262 +  20, center.Y - 353 + 200) == Color.FromArgb( 50,  50,  50);
            ret &= bitmap.GetPixel(center.X - 262 +  50, center.Y - 353 + 200) == Color.FromArgb(167, 155, 145);
            ret &= bitmap.GetPixel(center.X - 262 + 730, center.Y - 353 + 335) == Color.FromArgb( 50,  50,  50);
            ret &= bitmap.GetPixel(center.X - 262 + 730, center.Y - 353 + 550) == Color.FromArgb( 61,  47,  43);

            return ret;
        }

        private bool IsWarFinished(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return false;
            }

            bool ret = true;

            var w = bitmap.Width;
            var h = bitmap.Height;

            var center = new Point(bitmap.Size.Width / 2, bitmap.Size.Height / 2);

            // 戦績結果が画面に表示されているかどうか
            ret &= bitmap.GetPixel(center.X - 262 +  20, center.Y - 353 + 200) == Color.FromArgb( 50,  50,  50);
            ret &= bitmap.GetPixel(center.X - 262 +  50, center.Y - 353 + 200) == Color.FromArgb(167, 155, 145);
            ret &= bitmap.GetPixel(center.X - 262 + 730, center.Y - 353 + 335) == Color.FromArgb( 50,  50,  50);
            ret &= bitmap.GetPixel(center.X - 262 + 730, center.Y - 353 + 550) == Color.FromArgb( 61,  47,  43);

            return ret;
        }
    }
}
