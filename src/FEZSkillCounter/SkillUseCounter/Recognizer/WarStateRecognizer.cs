using SkillUseCounter.Entity;
using System;
using System.Drawing;

namespace SkillUseCounter.Recognizer
{
    internal class WarStateRecognizer
    {
        private enum WarState
        {
            Waiting,
            Waring,
        }

        private IRecognizer<Map> _mapRecognizer;

        private Map      _previousValidMap = Map.Empty;
        private WarState _previousState    = WarState.Waiting;

        // 戦争開始
        public event EventHandler<Map> WarStarted;

        // 戦争中断
        //   FOして別の戦場に入った場合、WarStartedの前に呼ばれる
        //   FOや回線落ちで戦場に復帰した場合は呼ばれない
        public event EventHandler<Map> WarCanceled;

        // 戦争終了
        public event EventHandler<Map> WarFinished;

        public WarStateRecognizer(IRecognizer<Map> mapRecognizer)
        {
            _mapRecognizer = mapRecognizer ?? throw new ArgumentNullException(nameof(mapRecognizer));
        }

        public void Reset()
        {
            _previousValidMap = Map.Empty;
            _previousState    = WarState.Waiting;
        }

        public void Report(Bitmap bitmap)
        {
            var state = _previousState;

            // Map取得
            var map = GetMap(bitmap);

            switch (_previousState)
            {
                case WarState.Waiting:
                    // 戦争待機中なら、戦争が開始したかどうかチェックする
                    if (IsDisplayCost(bitmap) && !map.IsEmpty())
                    {
                        state = WarState.Waring;
                        WarStarted?.Invoke(this, map);
                    }
                    break;

                case WarState.Waring:
                    // 戦争中なら、戦争が終了したかどうかチェックする
                    // 考慮事項：FO・死んだときやMAP名上にマウスカーソル
                    if (IsDisplayWarResult(bitmap) && !map.IsEmpty())
                    {
                        state = WarState.Waiting;
                        WarFinished?.Invoke(this, map);
                    }
                    // FOして別の戦場を入りなおしたとき、状態はAtWarのままのため、
                    // 一度Waitingに状態変更通知を投げて、即座に戦争状態に戻す
                    if (!map.IsEmpty() && map != _previousValidMap)
                    {
                        WarCanceled?.Invoke(this, map);
                        WarStarted?.Invoke(this, map);
                    }
                    break;

                default:
                    throw new Exception("invalid state");
            }

            // 状態更新
            _previousState = state;

            if (!map.IsEmpty())
            {
                _previousValidMap   = map;
            }
        }

        private Map GetMap(Bitmap bitmap)
        {
            return _mapRecognizer.Recognize(bitmap);
        }

        private bool IsDisplayCost(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return false;
            }

            bool ret = true;

            var w = bitmap.Width;
            var h = bitmap.Height;

            // [Cost 130 / 130] の"C"と"/"の部分で判別する
            ret &= bitmap.GetPixel(w - 441, h - 128) == Color.FromArgb(123, 123, 123);  // "C"
            ret &= bitmap.GetPixel(w - 375, h - 120) == Color.FromArgb(156, 156, 156);  // "/"

            return ret;
        }

        private bool IsDisplayWarResult(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return false;
            }

            bool ret = true;

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
