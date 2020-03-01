using FEZSkillCounter.Properties;
using FEZSkillCounter.UseCase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FEZSkillCounter.Model.Notificator
{
    public class EnchantSpellUseNotificator : IDisposable
    {
        private const int Invalid_Hp = -1;

        /// <summary>
        /// 戦争開始から通知を出すまでの待機時間
        /// </summary>
        public TimeSpan NotifyTimeSpan { get; set; }

        /// <summary>
        /// エンチャント未使用通知の有効
        /// </summary>
        public bool IsEnchantNotifyEnabled { get; set; }

        /// <summary>
        /// スペル未使用通知の有効
        /// </summary>
        public bool IsSpellNotifyEnabled { get; set; }

        private SoundPlayer _soundPlayer;
        private int _hp = Invalid_Hp;
        private CancellationTokenSource _cts = null;
        private WarEvents _warEvent = WarEvents.Invalid;

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

                if (_cts != null)
                {
                    _cts.Cancel(false);
                    _cts.Dispose();
                }
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
                return new SoundPlayer(Resources.enchant_notify);
            }
        }

        /// <summary>
        /// 戦争開始を報告
        /// </summary>
        public void ReportWarStarted()
        {
            // 戦争開始で別スレッドを立ち上げて通知準備
            if (_cts != null)
            {
                _cts.Cancel(false);
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();

            var token = _cts.Token;
            Task.Run(async () => await PlayNotifySoundIfSpellEhchantNotUsedAsync(token), token);
        }

        /// <summary>
        /// 現在のHPを報告
        /// </summary>
        /// <param name="hp"></param>
        public void ReportHp(int hp)
        {
            _hp = hp;
        }

        private async Task PlayNotifySoundIfSpellEhchantNotUsedAsync(CancellationToken token)
        {
            try
            {
                // 通知時間まで待機
                await Task.Delay(NotifyTimeSpan, token);

                // 現在のHPを確認して、
                // スペルまたはエンチャントが使用されていなければ通知
                if ((!IsSpellUsed(_hp)   && IsSpellNotifyEnabled) ||
                    (!IsEnchantUsed(_hp) && IsEnchantNotifyEnabled))
                {
                    PlayNotifySound();
                }
            }
            catch (OperationCanceledException)
            { }
            catch
            {
                // nop
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
            return hp >= 1100;
        }

        private bool IsEnchantUsed(int hp)
        {
            // HPエンチャント使用でHPが1000を超えているかチェック
            //
            // 1100の場合はスペル使用の可能性があるため除外
            // HPエンチャントで+100の可能性もあるが、確率的に低いため考慮しない。
            // また、HPスペルによるHP増加も判断できないため考慮しない。
            return (hp > 1000 && hp != 1100);
        }
    }
}
