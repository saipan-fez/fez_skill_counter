using FEZSkillCounter.Properties;
using FEZSkillCounter.UseCase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace FEZSkillCounter.Model.Notificator
{
    public class EnchantSpellUseNotificator : IDisposable
    {
        /// <summary>
        /// 戦争開始から通知を出すまでの待機時間
        /// </summary>
        public TimeSpan NotifyTimeSpan { get; }

        private DateTime? _warStartDateTime = null; // 戦争開始時間
        private bool _isNotified = false;
        private SoundPlayer _soundPlayer;

        /// <summary>
        /// </summary>
        /// <param name="notifySoundPath">通知で再生する音声ファイルパス</param>
        public EnchantSpellUseNotificator(string notifySoundPath)
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
        /// 現在の状態を通知し、必要であればユーザに書の使用を通知します4
        /// </summary>
        /// <param name="warEvents"></param>
        /// <param name="hp"></param>
        public void ReportCurrentStatusWithNotify(WarEvents warEvents, int hp)
        {
            // 戦争中じゃなければ通知対象外
            if (warEvents != WarEvents.WarStarted)
            {
                _warStartDateTime = null;
                _isNotified = false;
                return;
            }

            // 戦争が開始したら開始時間を保持
            if (!_warStartDateTime.HasValue)
            {
                _warStartDateTime = DateTime.Now;
            }

            // 未通知かつ、戦争開始から所定の時間経過している、
            // かつスペルまたはエンチャントが使用されていなければ通知
            if (!_isNotified && (DateTime.Now - _warStartDateTime) > NotifyTimeSpan)
            {
                if (!IsSpellUsed(hp) || !IsEnchantUsed(hp))
                {
                    PlayNotifySound();
                    _isNotified = true;
                }
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

        private bool IsSpellUsed(int hp)
        {
            // スペル使用時のHP+100ボーナスがあるかどうかチェック
            return hp >= 110;
        }

        private bool IsEnchantUsed(int hp)
        {
            // HPエンチャント使用時のHPが1000を超えているかチェック
            // なお、1100の場合はスペル使用の可能性があるため除外
            // (HPエンチャントで+100の可能性もあるが、確率的に低いため考慮しない)
            return (hp > 1000 && hp != 1100);
        }
    }
}
