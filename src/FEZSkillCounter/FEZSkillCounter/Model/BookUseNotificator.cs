using FEZSkillCounter.Properties;
using FEZSkillCounter.UseCase;
using SkillUseCounter.Entity;
using System;
using System.IO;
using System.Media;

namespace FEZSkillCounter.Model
{
    public class BookUseNotificator : IDisposable
    {
        /// <summary>
        /// 書の使用通知を開始する残り拠点ダメージ
        /// </summary>
        private const double BookUseNotificationThreashold = 0.5;

        /// <summary>
        /// 書の使用通知を実行する間隔
        /// </summary>
        private readonly TimeSpan BookUseNoticationTimeSpan = TimeSpan.FromMinutes(1);

        private DateTime? _previousNotificateDateTime = null;   // 前回通知を行った日時
        private SoundPlayer _soundPlayer;

        /// <summary>
        /// </summary>
        /// <param name="notifySoundPath">通知で再生する音声ファイルパス</param>
        public BookUseNotificator(string notifySoundPath)
        {
            _soundPlayer = CreateSoundPlayer(notifySoundPath);
        }

        public void Dispose()
        {
            try
            {
                _soundPlayer.Dispose();
            }
            catch
            { }
        }

        private SoundPlayer CreateSoundPlayer(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException();
                }

                return new SoundPlayer(path);
            }
            catch
            {
                // 失敗した場合はデフォルトの音声
                return new SoundPlayer(Resources.book_notify);
            }
        }

        /// <summary>
        /// 現在の状態をレポートし、必要であればユーザに書の使用を通知します
        /// </summary>
        /// <param name="warEvents"></param>
        /// <param name="isBookUsed"></param>
        /// <param name="keepDamage"></param>
        public void ReportCurrentStatusWithNotify(WarEvents warEvents, bool isBookUsed, KeepDamage keepDamage)
        {
            // 書を使用中なら通知の必要がないので対象外
            if (isBookUsed)
            {
                return;
            }

            // 戦争中じゃなければ通知対象外
            if (warEvents != WarEvents.WarStarted)
            {
                return;
            }

            // ゲージが一定以下でなければ通知対象外
            var min = Math.Min(keepDamage.AttackKeepDamage, keepDamage.DefenceKeepDamage);
            if (keepDamage.IsEmpty() || min > BookUseNotificationThreashold)
            {
                return;
            }

            // 初回の通知、または前回の通知から一定時間経過していれば通知する
            if (!_previousNotificateDateTime.HasValue ||
               (DateTime.Now - _previousNotificateDateTime.Value) > BookUseNoticationTimeSpan)
            {
                PlayNotifySound();

                _previousNotificateDateTime = DateTime.Now;
            }
        }

        private void PlayNotifySound()
        {
            try
            {
                _soundPlayer.Play();
            }
            catch
            { }
        }
    }
}
