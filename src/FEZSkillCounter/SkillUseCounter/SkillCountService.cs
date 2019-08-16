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
    public class SkillCountService
    {
        #region Event
        /// <summary>
        /// 処理速度(直近100回)を通知
        /// </summary>
        public event EventHandler<double> ProcessTimeUpdated;

        /// <summary>
        /// スキル一覧の更新通知
        /// </summary>
        public event EventHandler<Skill[]> SkillsUpdated;

        /// <summary>
        /// Powの更新通知
        /// </summary>
        public event EventHandler<int> PowUpdated;

        /// <summary>
        /// Powのデバフ(パワブレなど)更新通知
        /// </summary>
        public event EventHandler<PowDebuff[]> PowDebuffsUpdated;

        /// <summary>
        /// スキル使用を通知
        /// </summary>
        public event EventHandler<Skill> SkillUsed;

        /// <summary>
        /// 戦争開始を通知
        /// </summary>
        public event EventHandler<Map> WarStarted;

        /// <summary>
        /// 戦争中断を通知
        /// </summary>
        /// <remarks>
        /// FOや回線落ちした後、別の戦場に入った場合<see cref="WarStarted"/>>の前に呼ばれる。
        //  なお、戦場に復帰した場合は呼ばれない。
        /// </remarks>
        public event EventHandler<Map> WarCanceled;

        /// <summary>
        /// 戦争終了を通知
        /// </summary>
        public event EventHandler<Map> WarFinished;
        #endregion

        private PreRecognizer                      _preRecognizer            = new PreRecognizer();
        private WarStateRecognizer                 _warStateRecognizer       = new WarStateRecognizer(new MapRecognizer());
        private IResettableRecognizer<Skill[]>     _skillArrayRecognizer     = new SkillArrayRecognizer();
        private IResettableRecognizer<PowDebuff[]> _powDebuffArrayRecognizer = new PowDebuffArrayRecognizer();
        private IResettableRecognizer<int>         _powRecognizer            = new PowRecognizer();

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

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Logger.WriteException(e.Exception);
            };

            _skillArrayRecognizer.Updated     += (_, e) => SkillsUpdated?.Invoke(this, e);
            _powRecognizer.Updated            += (_, e) => PowUpdated?.Invoke(this, e);
            _powDebuffArrayRecognizer.Updated += (_, e) => PowDebuffsUpdated?.Invoke(this, e);
            _warStateRecognizer.WarStarted    += (_, e) =>
            {
                Logger.WriteLine("War started.");

                // 開始時に各種数値をリセットする
                Reset();
                WarStarted?.Invoke(this, e);
            };
            _warStateRecognizer.WarCanceled += (_, e) =>
            {
                Logger.WriteLine("War canceled.");
                WarCanceled?.Invoke(this, e);
            };
            _warStateRecognizer.WarFinished += (_, e) =>
            {
                Logger.WriteLine("War finished.");
                WarFinished?.Invoke(this, e);
            };
        }

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
                _cts = new CancellationTokenSource();
                var token = _cts.Token;
                _task = Task.Run(() => { Run(token); }, token);

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
        private void Reset()
        {
            Logger.WriteLine("SkillCountService reset.");

            _skillArrayRecognizer.Reset();
            _powDebuffArrayRecognizer.Reset();
            _powRecognizer.Reset();
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
                        ProcessTimeUpdated?.Invoke(this, processTimes.Average());
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

                // 現在のPow・スキル・デバフを取得
                var pow        = _powRecognizer.Recognize(screenShot.Image);
                var skills     = _skillArrayRecognizer.Recognize(screenShot.Image);
                var powDebuffs = _powDebuffArrayRecognizer.Recognize(screenShot.Image);

                // 取得失敗していれば終了
                if (pow         == PowRecognizer.InvalidPow ||
                    //skills      == SkillArrayRecognizer.InvalidSkills ||
                    powDebuffs  == PowDebuffArrayRecognizer.InvalidPowDebuffs)
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
                        usedSkill,
                        null,
                        null);
                }
            }

            return true;
        }
    }
}
