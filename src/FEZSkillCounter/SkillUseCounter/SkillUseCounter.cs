using FEZSkillUseCounter;
using FEZSkillUseCounter.Algorithm;
using FEZSkillUseCounter.Entity;
using FEZSkillUseCounter.Recognizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SkillUseCounter
{
    /// <summary>
    /// スキル使用をカウント
    /// </summary>
    public class SkillCountService
    {
        private PreRecognizer            _preRecognizer            = new PreRecognizer();
        private WarStateRecognizer       _warStateRecognizer       = new WarStateRecognizer();
        private SkillArrayRecognizer     _skillArrayRecognizer     = new SkillArrayRecognizer();
        private PowDebuffArrayRecognizer _powDebuffArrayRecognizer = new PowDebuffArrayRecognizer();
        private PowRecognizer            _powRecognizer            = new PowRecognizer();

        private FEZScreenShotStorage _screenShotStorage   = new FEZScreenShotStorage();
        private SkillUseAlgorithm    _skillCountAlgorithm = new SkillUseAlgorithm();

        private CancellationTokenSource _cts  = null;
        private Task                    _task = null;

        public event EventHandler<double>      FpsUpdated;
        public event EventHandler<Skill[]>     SkillsUpdated;
        public event EventHandler<int>         PowUpdated;
        public event EventHandler<PowDebuff[]> PowDebuffsUpdated;
        public event EventHandler<Skill>       SkillCountIncremented;

        public SkillCountService()
        {
            _skillArrayRecognizer.Updated     += (_, e) => SkillsUpdated?.Invoke(this, e);
            _powRecognizer.Updated            += (_, e) => PowUpdated?.Invoke(this, e);
            _powDebuffArrayRecognizer.Updated += (_, e) => PowDebuffsUpdated?.Invoke(this, e);
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
            }
            catch
            {
                // TODO: errorハンドリング
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
                _cts.Cancel(false);
                _task.Wait();
            }
            catch
            {
                // TODO: errorハンドリング
                throw;
            }
            finally
            {
                _cts.Dispose();
                _cts  = null;
                _task = null;
            }
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
                        FpsUpdated?.Invoke(this, processTimes.Average());
                        processTimes.Clear();
                    }

                    // 処理が失敗している場合は大抵即終了している。
                    // ループによるCPU使用率を抑えるためにwait
                    if (!result)
                    {
                        Thread.Sleep(30);
                    }
                }
                catch (OperationCanceledException)
                { }
                catch
                {
                    // TODO: errorハンドリング
                    throw;
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

                // 現在のPow・スキル・デバフを取得
                var pow        = _powRecognizer.Recognize(screenShot.Image);
                var skills     = _skillArrayRecognizer.Recognize(screenShot.Image);
                var powDebuffs = _powDebuffArrayRecognizer.Recognize(screenShot.Image);

                // 選択中スキルを取得
                var activeSkill = skills.FirstOrDefault(x => x.IsActive);

                // 取得失敗していれば終了
                if (pow         == PowRecognizer.InvalidPow ||
                    skills      == SkillArrayRecognizer.InvalidSkills ||
                    powDebuffs  == PowDebuffArrayRecognizer.InvalidPowDebuffs ||
                    activeSkill == default(Skill))
                {
                    return false;
                }

                // スキルを使ったかどうかチェック
                var isSkillUsed = _skillCountAlgorithm.IsSkillUsed(
                    screenShot.TimeStamp,
                    pow,
                    activeSkill,
                    powDebuffs);

                // スキルを使っていれば更新通知
                if (isSkillUsed)
                {
                    SkillCountIncremented?.BeginInvoke(
                        this,
                        activeSkill,
                        null,
                        null);
                }
            }

            return true;
        }
    }
}
