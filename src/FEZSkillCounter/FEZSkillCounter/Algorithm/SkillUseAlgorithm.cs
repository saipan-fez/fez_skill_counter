using FEZSkillCounter.Entity;
using FEZSkillCounter.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FEZSkillCounter.Algorithm
{
    public class SkillUseAlgorithm
    {
        /// <summary>
        /// フレーム毎に回復する可能性のある値の閾値
        /// </summary>
        /// <remarks>
        /// Pow回復中に、パワブレまたはスキル使用によるPow減少が起きると、
        /// 本来 20 消費するはずのところが、回復によって 15 といったことになりうる。
        /// フレーム取得タイミングが一定でないため、回復量も一定ではない。
        /// そのためフレーム毎で回復しうる閾値を設ける。
        /// </remarks>
        private const int PowRegenerateThreashold = 5;

        public class DebuffData
        {
            public int DebuffId { get; set; }
            public long TimeStamp { get; set; }
            public int[] Pow { get; set; }
        }

        private long _previousTimeStamp;
        private int _previousPow;
        private PowDebuff[] _previousPowDebuff;

        private List<DebuffData> _debuffList = new List<DebuffData>();
        public IReadOnlyList<DebuffData> DebuffList
        {
            get
            {
                return _debuffList;
            }
        }

        public SkillUseAlgorithm()
        {
            Reset();
        }

        public void Reset()
        {
            _previousTimeStamp = long.MinValue;
            _previousPow = int.MinValue;
            _previousPowDebuff = null;
            _debuffList.Clear();
        }

        public bool IsSkillUsed(long timeStamp, int pow, Skill activeSkill, PowDebuff[] powDebuff)
        {
            if (timeStamp < 0 || pow < 0)
            {
                throw new ArgumentException($"{nameof(timeStamp)}:{timeStamp} {nameof(pow)}:{pow}");
            }

            /* パワーブレイク判定アルゴリズムについて
             *
             *  ■概要
             *  スキルの使用はPowの減少によって判断する。
             *  しかし、Powの減少はスキルの使用以外にパワーブレイク(以下パワブレ)によっても減少する。
             *  そこで、Powの減少がパワブレによるものかどうか判定し、
             *  パワブレによるPow減少でなければスキル使用と判断する。
             *
             *  ■前提
             *  1. 複数Pow減少デバフが存在しないものとする
             *       → Pow減少デバフは現状スカウトのパワブレのみのため
             *  2. Pow回復は徐々に増加する
             *       → 50から65に回復するとき、53, 55, 60…といった具合に増加する。
             *          増加量は取得するフレームの間隔に依存するため一定ではない。
             *  3. Pow減少(スキル使用・パワブレ)は一気に減少する
             *       → 回復と異なり減少は一気に行われる
             *
             *  ■アルゴリズム
             *  前回と今回のPowBuffの一覧を比較し、新たにパワブレが追加されていれば、
             *  前回から今回までの間にパワブレをもらったということになる。
             *  パワブレはスキルをもらってから、3, 6, 9 ... 24秒経過後(3秒間隔)でPowが減少するため、
             *  1回目のPow減少は "前回のタイムスタンプ + 3秒後"、
             *  2回目のPow減少は "前回のタイムスタンプ + 6秒後"、
             *  ・
             *  ・
             *  ・
             *  がそれぞれ最短のPow減少タイミングとなる。
             *  パワブレをもらったタイミングで減少する最短タイミングをリストに追加し、
             *  以降はその最短タイミングを超えていないかチェックし、
             *  超えていればその時点でのPow減少がパワブレによるものと判定する。
             */
            var isSkillUsed = false;

            // 初回実行時は判定しようがないので終了
            if (_previousPowDebuff == null || _previousPow == int.MinValue)
            {
                goto Finish;
            }

            // 今回新たに発生したデバフ(今回と前回の差集合)を取得
            var enterPowDebuffs = powDebuff.Except(_previousPowDebuff, x => x.Id);

            // 新たに発生したデバフがあればリストに追加
            if (enterPowDebuffs.Any())
            {
                foreach (var debuff in enterPowDebuffs)
                {
                    for (int i = 0; i < debuff.EffectCount; i++)
                    {
                        _debuffList.Add(new DebuffData()
                        {
                            DebuffId = debuff.Id,
                            TimeStamp = (_previousTimeStamp + (i + 1) * debuff.EffectTimeSpan.Ticks),
                            Pow = debuff.Pow.Clone() as int[]
                        });
                    }
                }

                // タイムスタンプの昇順でソート(＝ 直近のもの)
                _debuffList = _debuffList.OrderBy(x => x.TimeStamp).ToList();
            }

            if (pow >= _previousPow)
            {
                // 以下の場合はスキルを使用していないため終了
                // ・前回と同じPow
                // ・前回よりPowが多い(回復した)
                goto CheckLeaveDebuff;
            }

            // 今回の消費Pow
            var powDiff = _previousPow - pow;

            // 今回のPow減少デバフの候補一覧を取得
            var list = _debuffList.Where(x => timeStamp > x.TimeStamp).ToArray();
            if (list.Any())
            {
                // 今回のデバフによる消費Powとして考えうるもの一覧
                // TODO: デバフが1つ(パワブレ)前提の作りになっている。
                //       2つ以上デバフが存在する場合は、デバフによる消費Powの取りうる値をすべて出す必要がある。
                //       対応すると無駄に複雑な処理となるため除外する。
                var debuffPowSum = list.First().Pow;

                // Pow回復とデバフによるPow消費が同時に発生した際を考慮し、
                // 回復しうる閾値以下であればデバフによるPow消費と判断する。
                // なお、上記にさらにスキル使用によるPow消費が同時に発生しうるが、
                // こちらは滅多にないため考慮しない。
                // TODO: その場合でも以降問題なく動作するようにする(現状ではおそらくバグる)
                if (debuffPowSum.Any(x => (x - powDiff) < PowRegenerateThreashold))
                {
                    Logger.WriteLine($"-------------------------");
                    Logger.WriteLine($"Detected to PowDebuff.");
                    Logger.WriteLine($"[previous]");
                    Logger.WriteLine($"     time:{_previousTimeStamp} pow:{_previousPow} debuff:{string.Join(",", _previousPowDebuff.Select(x => x.Name))}");
                    Logger.WriteLine($"[current]");
                    Logger.WriteLine($"     time:{timeStamp} pow:{pow} debuff:{string.Join(",", powDebuff.Select(x => x.Name))}");

                    // デバフによるPow消費があったため一覧から削除
                    foreach (var d in list)
                    {
                        _debuffList.Remove(d);
                    }
                }
            }
            else
            {
                if (activeSkill.Pow.Any(x => (x - powDiff) < PowRegenerateThreashold))
                {
                    Logger.WriteLine($"-------------------------");
                    Logger.WriteLine($"Detected to use skill. skill:{activeSkill.Name}");
                    Logger.WriteLine($"[previous]");
                    Logger.WriteLine($"     time:{_previousTimeStamp} pow:{_previousPow} debuff:{string.Join(",", _previousPowDebuff.Select(x => x.Name))}");
                    Logger.WriteLine($"[current]");
                    Logger.WriteLine($"     time:{timeStamp} pow:{pow} debuff:{string.Join(",", powDebuff.Select(x => x.Name))}");

                    // Pow回復とスキル使用によるPow消費が同時に発生した際を考慮し、
                    // 回復しうる閾値以下であればスキル使用によるPow消費と判断する。
                    isSkillUsed = true;
                }
            }

    CheckLeaveDebuff:
            // 今回消失したもの(前回と今回の差集合)を取得し、
            // 消失されているものの一覧に該当デバフがあれば削除する。
            // 通常のPow減少などでは常にfalseとなるが、
            // パワブレ状態でデッドしてしまうとデバフが削除されるが、
            // 一覧には残り続けるため、そのための対応としてチェック→削除する。
            var leavePowDebuffs = _previousPowDebuff.Except(powDebuff, x => x.Id);
            if (leavePowDebuffs.Any())
            {
                Logger.WriteLine($"-------------------------");
                Logger.WriteLine($"leavePowDebuffs.Any() is true.");
                Logger.WriteLine($"[previous]");
                Logger.WriteLine($"    {string.Join(",", _previousPowDebuff.Select(x => x.Name))}");
                Logger.WriteLine($"[current]");
                Logger.WriteLine($"    {string.Join(",", powDebuff.Select(x => x.Name))}");

                _debuffList.RemoveAll(x => leavePowDebuffs.Any(y => y.Id == x.DebuffId));
            }

    Finish:
            // 今回の値を保持
            _previousTimeStamp = timeStamp;
            _previousPow       = pow;
            _previousPowDebuff = powDebuff;

            return isSkillUsed;
        }
    }
}
