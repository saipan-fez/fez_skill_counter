using SkillUseCounter.Algorithm;
using SkillUseCounter.Entity;
using SkillUseCounter.Recognizer;
using SkillUseCounter.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillUseCounter
{
    /// <summary>
    /// スキル使用を通知するサービス
    /// </summary>
    public class SkillCountService : IDisposable
    {
        #region Event
        /// <summary>
        /// 処理速度(直近100回)を通知
        /// </summary>
        public event EventHandler<ProcessTimeUpdatedEventArgs> ProcessTimeUpdated;

        /// <summary>
        /// スキル一覧の更新通知
        /// </summary>
        public event EventHandler<SkillsUpdatedEventArgs> SkillsUpdated;

        /// <summary>
        /// Powの更新通知
        /// </summary>
        public event EventHandler<PowUpdatedEventArgs> PowUpdated;

        /// <summary>
        /// Hpの更新通知
        /// </summary>
        public event EventHandler<HpUpdatedEventArgs> HpUpdated;

        /// <summary>
        /// Powのデバフ(パワブレなど)更新通知
        /// </summary>
        public event EventHandler<PowDebuffsUpdatedEventArgs> PowDebuffsUpdated;

        /// <summary>
        /// キープへのダメージ量変更通知
        /// </summary>
        public event EventHandler<KeepDamageUpdatedEventArgs> KeepDamageUpdated;

        /// <summary>
        /// 書の使用状態変更通知
        /// </summary>
        public event EventHandler<BookUsesUpdatedEventArgs> BookUsesUpdated;

        /// <summary>
        /// スキル使用を通知
        /// </summary>
        public event EventHandler<SkillUsedEventArgs> SkillUsed;

        /// <summary>
        /// 戦争開始を通知
        /// </summary>
        public event EventHandler<WarStartedEventArgs> WarStarted;

        /// <summary>
        /// 戦争中断を通知
        /// </summary>
        /// <remarks>
        /// FOや回線落ちした後、別の戦場に入った場合<see cref="WarStarted"/>>の前に呼ばれる。
        //  なお、戦場に復帰した場合は呼ばれない。
        /// </remarks>
        public event EventHandler<WarCanceledEventArgs> WarCanceled;

        /// <summary>
        /// 戦争終了を通知
        /// </summary>
        public event EventHandler<WarFinishedEventArgs> WarFinished;
        #endregion

        private PreRecognizer                      _preRecognizer            = new PreRecognizer();
        private WarStateRecognizer                 _warStateRecognizer       = new WarStateRecognizer(new MapRecognizer());
        private IResettableRecognizer<Skill[]>     _skillArrayRecognizer     = new SkillArrayRecognizer();
        private IResettableRecognizer<PowDebuff[]> _powDebuffArrayRecognizer = new PowDebuffArrayRecognizer();
        private IResettableRecognizer<int>         _powRecognizer            = new PowRecognizer();
        private IResettableRecognizer<int>         _hpRecognizer             = new HpRecognizer();
        private IResettableRecognizer<KeepDamage>  _keepDamageRecognizer     = new KeepDamageRecognizer();
        private IResettableRecognizer<bool>        _bookUseRecognizer        = new BookUseRecognizer();

        private FEZScreenShotStorage               _screenShotStorage        = new FEZScreenShotStorage();
        private SkillUseAlgorithm                  _skillCountAlgorithm      = new SkillUseAlgorithm();

        private CancellationTokenSource _cts  = null;
        private Task                    _task = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SkillCountService()
        {
            SkillStorage.Create();
            PowStorage.Create();
            MapStorage.Create();

            _skillArrayRecognizer.Updated     += (_, e) => SkillsUpdated?.Invoke(this, new SkillsUpdatedEventArgs(e));
            _powRecognizer.Updated            += (_, e) => PowUpdated?.Invoke(this, new PowUpdatedEventArgs(e));
            _hpRecognizer.Updated             += (_, e) => HpUpdated?.Invoke(this, new HpUpdatedEventArgs(e));
            _powDebuffArrayRecognizer.Updated += (_, e) => PowDebuffsUpdated?.Invoke(this, new PowDebuffsUpdatedEventArgs(e));
            _keepDamageRecognizer.Updated     += (_, e) => KeepDamageUpdated?.Invoke(this, new KeepDamageUpdatedEventArgs(e));
            _bookUseRecognizer.Updated        += (_, e) => BookUsesUpdated?.Invoke(this, new BookUsesUpdatedEventArgs(e));
            _warStateRecognizer.WarStarted    += (_, e) =>
            {
                Logger.WriteLine("War started.");
                WarStarted?.Invoke(this, new WarStartedEventArgs(e));
            };
            _warStateRecognizer.WarCanceled += (_, e) =>
            {
                Logger.WriteLine("War canceled.");
                WarCanceled?.Invoke(this, new WarCanceledEventArgs(e));
            };
            _warStateRecognizer.WarFinished += (_, e) =>
            {
                Logger.WriteLine("War finished.");
                WarFinished?.Invoke(this, new WarFinishedEventArgs(e));
            };
        }

        #region IDisposable 
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                Stop();

                if (_cts != null)
                {
                    _cts.Dispose();
                    _cts = null;
                }
            }
            finally
            {
                _disposed = true;
            }
        }
        #endregion

        /// <summary>
        /// スキル使用の監視開始
        /// </summary>
        public void Start()
        {
            if (_cts != null)
            {
                return;
            }

            try
            {
                _cts      = new CancellationTokenSource();
                var token = _cts.Token;
                _task     = Task.Run(() => { Run(token); }, token);

                Logger.WriteLine("SkillCountService started.");
            }
            catch
            {
                Stop();
                throw;
            }
        }

        /// <summary>
        /// スキル使用の監視停止
        /// </summary>
        public void Stop()
        {
            if (_cts == null)
            {
                return;
            }

            try
            {
                Logger.WriteLine("SkillCountService stopped.");

                _cts.Cancel(false);
                _task.Wait();
            }
            catch
            {
                throw;
            }
            finally
            {
                _cts.Dispose();
                _cts  = null;
                _task = null;
            }
        }

        /// <summary>
        /// スキル使用の監視状況をリセット
        /// </summary>
        public void Reset()
        {
            Logger.WriteLine("SkillCountService reset.");

            _warStateRecognizer.Reset();
            _skillArrayRecognizer.Reset();
            _powDebuffArrayRecognizer.Reset();
            _keepDamageRecognizer.Reset();
            _bookUseRecognizer.Reset();
            _powRecognizer.Reset();
            _hpRecognizer.Reset();
        }

        private void Run(CancellationToken token)
        {
            var stopwatch    = Stopwatch.StartNew();
            var processTimes = new List<long>(100);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // 解析
                    var start  = stopwatch.ElapsedMilliseconds;
                    var result = Analyze();
                    var end    = stopwatch.ElapsedMilliseconds;

                    // 解析時間更新
                    processTimes.Add(end - start);
                    if (processTimes.Count > 100)
                    {
                        ProcessTimeUpdated?.Invoke(this, new ProcessTimeUpdatedEventArgs(processTimes.Average()));
                        processTimes.Clear();
                    }

                    // 処理が失敗している場合は大抵即終了している。
                    // ループによるCPU使用率を抑えるためにwait
                    var waitTime = 33 - (int)(end - start);
                    if (waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                    }
                }
                catch (OperationCanceledException)
                { }
                catch
                {
                    // 毎回出るエラーだった場合、エラーが出続けることになるため
                    // エラー発生時は終了する。
                    break;
                }
            }
        }

        private bool Analyze()
        {
            using (var screenShot = _screenShotStorage.Shoot())
            {
                // 解析可能か確認
                if (!_preRecognizer.Recognize(screenShot.Image))
                {
                    return false;
                }

                // 戦争の状況を登録
                _warStateRecognizer.Report(screenShot.Image);

                // 現在のPow・スキル・デバフ・キープダメージ・倍書の使用状況を取得
                var pow        = _powRecognizer.Recognize(screenShot.Image);
                var hp         = _hpRecognizer.Recognize(screenShot.Image);
                var skills     = _skillArrayRecognizer.Recognize(screenShot.Image);
                var powDebuffs = _powDebuffArrayRecognizer.Recognize(screenShot.Image);
                var keepDamage = _keepDamageRecognizer.Recognize(screenShot.Image);
                var isBookUsed = _bookUseRecognizer.Recognize(screenShot.Image);

                // 取得失敗していれば終了
                //   キープダメージ・倍書の使用状況は取得有無がスキル使用に影響がないためチェックしない
                if (hp         == HpRecognizer.InvalidHp ||
                    pow        == PowRecognizer.InvalidPow ||
                    powDebuffs == PowDebuffArrayRecognizer.InvalidPowDebuffs)
                {
                    return false;
                }

                // スキルを使ったかどうかチェック
                var usedSkill = _skillCountAlgorithm.RecognizeUsedSkill(
                    screenShot.TimeStamp,
                    pow,
                    skills,
                    powDebuffs);

                // スキルを使っていれば更新通知
                if (usedSkill != null)
                {
                    SkillUsed?.BeginInvoke(
                        this,
                        new SkillUsedEventArgs(usedSkill),
                        null,
                        null);
                }
            }

            return true;
        }
    }
}
