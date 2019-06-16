using FEZSkillCounter.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FEZSkillCounter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (_, __) => Task.Run(async () => await ProcAsync());
            SkillUpdated += MainWindow_SkillUpdated;
            SkillCountIncremented += MainWindow_SkillCountIncremented;
            PowUpdated += MainWindow_MpUpdated;

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {

            };
        }

        private void MainWindow_MpUpdated(object sender, int pow)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                PowText.Text = pow.ToString();
            })));
        }

        private List<SkillCount> _skillList = new List<SkillCount>();

        private void MainWindow_SkillUpdated(object sender, Skill[] skills)
        {
            bool requireUpdate = false;
            foreach (var s in skills)
            {
                if (!_skillList.Any(x => x.Skill.Name == s.Name))
                {
                    _skillList.Add(new SkillCount(s));
                    requireUpdate = true;
                }
            }

            if (requireUpdate)
            {
                UpdateSkillText();
            }
        }

        private void MainWindow_SkillCountIncremented(object sender, Skill skill)
        {
            var skillCount = _skillList.FirstOrDefault(x => x.Skill.Name == skill.Name);
            if (skillCount != null)
            {
                skillCount.Increment();

                UpdateSkillText();
            }
        }

        private void UpdateSkillText()
        {
            var text = string.Join(Environment.NewLine, _skillList.Select(x => x.Skill.Name + ":" + x.Count));

            Dispatcher.BeginInvoke(((Action)(() =>
            {
                SkillText.Text = text;
            })));
        }

        public class SkillCount
        {
            public Skill Skill { get; }
            public int Count { get; private set; }

            public SkillCount(Skill skill)
            {
                Skill = skill;
                Count = 0;
            }

            public void Increment()
            {
                Count++;
            }
        }

        // ------------------------------------------------------------
        // ------------------------------------------------------------
        // ------------------------------------------------------------
        // ------------------------------------------------------------
        // ------------------------------------------------------------

        public enum Status
        {
            War,        // 戦争中
            Waiting,    // 戦争中ではない
        }

        private SkillOcr _skillOcr = new SkillOcr();
        private FEZScreenShooter _screenShooter = new FEZScreenShooter();
        private Stopwatch _stopwatch = Stopwatch.StartNew();
        private SkillCountAlgorithm skillCountAlgorithm = new SkillCountAlgorithm();

        private Status _status = Status.Waiting;
        private int _currentPow = int.MaxValue;
        private Skill[] _currentSkills;

        private event EventHandler<int> PowUpdated;
        private event EventHandler<Skill[]> SkillUpdated;
        private event EventHandler<Skill> SkillCountIncremented;

        public class Stack<T>
        {
            public T[] data;
            int pos = 0;
            int maxPos;

            public Stack(int size)
            {
                data = new T[size];
                maxPos = size - 1;
            }

            public void Add(T o)
            {
                if (pos < maxPos)
                {
                    pos++;
                }

                for (int i = 0; i < pos; i++)
                {
                    data[i] = data[i + 1];
                }

                data[pos] = o;
            }
        }

        long processTotalTime  = 0;
        long processTotalCount = 0;

        private async Task ProcAsync()
        {
            while (true)
            {
                //Thread.Sleep(0);

                // TODO: 非戦争→戦争中に切り替わったら使用回数をクリア

                var start = _stopwatch.ElapsedMilliseconds;

                using (var screenShot = _screenShooter.Shoot())
                {
                    // 解析可能か確認
                    if (!CanAnalyze(screenShot.Image))
                    {
                        continue;
                    }

                    var previousPow       = _currentPow;
                    var previousSkills    = _currentSkills;
                    PowDebuff[] powDebuff = null;

                    // 現在のPowを取得
                    _currentPow = GetPow(screenShot.Image);

                    // スキル取得
                    _currentSkills = GetSkills(screenShot.Image);

                    // デバフ取得
                    powDebuff = GetPowDebuffs(screenShot.Image);

                    //await Task.WhenAll(
                    //    Task.Run(() =>
                    //    {
                    //        // 現在のPowを取得
                    //        _currentPow = GetPow(screenShot.Image);
                    //    }),
                    //    Task.Run(() =>
                    //    {
                    //        // スキル取得
                    //        _currentSkills = GetSkills(screenShot.Image);
                    //    }),
                    //    Task.Run(() =>
                    //    {
                    //        // デバフ取得
                    //        powDebuff = GetPowDebuffs(screenShot.Image);
                    //    }));

                    // 取得失敗していれば終了
                    if (_currentPow == int.MaxValue ||
                        _currentSkills == null ||
                        powDebuff == null)
                    {
                        continue;
                    }

                    // Powが更新されていれば通知
                    if (previousPow != int.MaxValue)
                    {
                        PowUpdated?.BeginInvoke(
                            this,
                            _currentPow,
                            null,
                            null);
                    }

                    // スキルが更新されていれば通知
                    if (previousSkills == null ||
                        !_currentSkills.SequenceEqual(previousSkills, x => x.Name))
                    {
                        SkillUpdated?.BeginInvoke(
                            this,
                            _currentSkills,
                            null,
                            null);
                    }

                    // 選択中スキルを取得
                    var activeSkill = _currentSkills.FirstOrDefault(x => x.IsActive);

                    // スキルを使ったかどうかチェック
                    var isSkillUsed = skillCountAlgorithm.IsSkillUsed(
                            screenShot.TimeStamp,
                            _currentPow,
                            activeSkill,
                            powDebuff);

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

                var end = _stopwatch.ElapsedMilliseconds;

                processTotalTime += (end - start);
                processTotalCount++;

                if (processTotalCount % 30 == 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ProcessAvgTimeText.Text = (processTotalTime / processTotalCount).ToString();
                    });
                }
            }
        }

        private bool IsWarStarted(Bitmap bitmap)
        {
            // 「戦闘開始」が画面に表示されているかどうかで判定する
            // ※途中参戦であっても参戦直後に表示される

            return false;
        }

        private bool IsWarFinished(Bitmap bitmap)
        {
            // 戦績結果が画面に表示されているかどうか

            return false;
        }

        private bool CanAnalyze(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return false;
            }

            var w = bitmap.Width;
            var h = bitmap.Height;

            bool ret = true;

            // 画面右上のスキル枠があるかどうか
            ret &= bitmap.GetPixel(w - 47, 10) == Color.FromArgb(178, 186, 192);
            ret &= bitmap.GetPixel(w - 47, 11) == Color.FromArgb(168, 106, 121);

            // 画面右下にHP/Pow枠があるかどうか
            ret &= bitmap.GetPixel(w - 315, h - 54) == Color.FromArgb(168, 160, 158);
            ret &= bitmap.GetPixel(w - 314, h - 54) == Color.FromArgb(29, 27, 26);

            return ret;
        }

        private int GetPow(Bitmap bitmap)
        {
            var pow = int.MaxValue;

            var w = bitmap.Width;
            var h = bitmap.Height;
            var f = PixelFormat.Format24bppRgb;

            using (var houndredsPlace = bitmap.Clone(new Rectangle(w - 143, h - 67, 7, 10), f))     // 百の位
            using (var tensPlace      = bitmap.Clone(new Rectangle(w - 135, h - 67, 7, 10), f))     // 十の位
            using (var onesPlace      = bitmap.Clone(new Rectangle(w - 127, h - 67, 7, 10), f))     // 一の位
            {
                var houndreds = GetPowNum(houndredsPlace);
                var tens      = GetPowNum(tensPlace);
                var ones      = GetPowNum(onesPlace);

                if (houndreds != int.MaxValue &&
                    tens != int.MaxValue &&
                    ones != int.MaxValue)
                {
                    pow = houndreds * 100 + tens * 10 + ones;
                }
            }

            return pow;
        }

        private int GetPowNum(Bitmap bitmap)
        {
            // 指定された位置の色から数値
            var cmpColor1 = Color.FromArgb(221, 221, 221).ToArgb();
            if (bitmap.GetPixel(3, 2).ToArgb() == cmpColor1)
            {
                return 0;
            }
            if (bitmap.GetPixel(3, 3).ToArgb() == cmpColor1)
            {
                return 1;
            }
            if (bitmap.GetPixel(1, 8).ToArgb() == cmpColor1)
            {
                return 2;
            }
            if (bitmap.GetPixel(4, 3).ToArgb() == cmpColor1)
            {
                return 4;
            }
            if (bitmap.GetPixel(1, 4).ToArgb() == cmpColor1)
            {
                return 5;
            }
            if (bitmap.GetPixel(2, 2).ToArgb() == cmpColor1)
            {
                return 6;
            }
            if (bitmap.GetPixel(6, 1).ToArgb() == cmpColor1)
            {
                return 7;
            }
            if (bitmap.GetPixel(6, 6).ToArgb() == cmpColor1)
            {
                return 8;
            }
            if (bitmap.GetPixel(1, 2).ToArgb() == cmpColor1)
            {
                return 9;
            }

            var cmpColor2 = Color.FromArgb(170, 170, 170).ToArgb();
            if (bitmap.GetPixel(5, 5).ToArgb() == cmpColor2)
            {
                return 3;
            }

            // Pow=90での百の位など数字が無い画像だった場合
            var cmpColor3 = Color.FromArgb(16, 10, 10).ToArgb();
            if (bitmap.GetPixel(0, 0).ToArgb() == cmpColor3)
            {
                return 0;
            }

            // 上記に当てはまらないものはエラーとする
            return int.MaxValue;
        }

        // 各スキルが描かれている位置 (x座標のみ画像右側からの相対座標)
        private Rectangle[] SkillRectTable = new Rectangle[]
        {
            new Rectangle(-30, 22, 12, 12),
            new Rectangle(-30, 54, 12, 12),
            new Rectangle(-30, 86, 12, 12),
            new Rectangle(-30, 118, 12, 12),
            new Rectangle(-30, 150, 12, 12),
            new Rectangle(-30, 182, 12, 12),
            new Rectangle(-30, 214, 12, 12),
            new Rectangle(-30, 246, 12, 12),
        };

        private Skill[] GetSkills(Bitmap bitmap)
        {
            Skill[] skills = new Skill[SkillRectTable.Length];

            for (int i = 0; i < SkillRectTable.Length; i++)
            {
                var r = SkillRectTable[i];
                r.X = bitmap.Width + r.X;

                using (var b = bitmap.Clone(r, PixelFormat.Format24bppRgb))
                {
                    skills[i] = _skillOcr.Process(b);
                }
            }

            return skills;
        }

        /// <summary>
        /// 右下アイコンの最大個数
        /// </summary>
        /// <remarks>
        /// 最大個数は未調査
        /// </remarks>
        private const int MaxPowerDebuffCount = 10;

        // パワーブレイクかどうか判定する色
        private readonly int PowerBreakCmpColor1 = Color.FromArgb(30, 125, 183).ToArgb();
        private readonly int PowerBreakCmpColor2 = Color.FromArgb(49, 134, 187).ToArgb();

        // パワーブレイク
        //   Lv1-3で減少Powが異なる
        private readonly PowDebuff PowerBreak = 
            new PowDebuff("パワーブレイク", new int[]{ -15, -20, -25 }, 8, TimeSpan.FromSeconds(3));

        private PowDebuff[] GetPowDebuffs(Bitmap bitmap)
        {
            for (int i = 0; i < MaxPowerDebuffCount; i++)
            {
                var x1 = bitmap.Width  - 28 - (32 * i);
                var x2 = bitmap.Width  - 34 - (32 * i);
                var y  = bitmap.Height - 20;

                if (bitmap.GetPixel(x1, y).ToArgb() == PowerBreakCmpColor1 &&
                    bitmap.GetPixel(x2, y).ToArgb() == PowerBreakCmpColor2)
                {
                    return new PowDebuff[]
                    {
                        PowerBreak
                    };
                }
            }

            return new PowDebuff[0];
        }
    }

    public class Skill
    {
        public string Name { get; }
        public int[] Pow { get; }
        public bool IsActive { get; }

        public Skill()
        { }

        public Skill(string name, int[] pow, bool isActive)
        {
            Name = name;
            Pow = pow;
            IsActive = isActive;
        }

        public static Skill CreateFromResourceFileName(string resourceName, params int[] pow)
        {
            var name = resourceName
                .Replace("Cestus_", "")
                .Replace("Fencer_", "")
                .Replace("Scout_", "")
                .Replace("Sorcerer_", "")
                .Replace("Warrior_", "")
                .Replace("_S", "")
                .Replace("_D", "");

            var isActive = (resourceName.IndexOf("_S") != -1);

            return new Skill(name, pow, isActive);
        }
    }

    public class PowDebuff
    {
        private static int idSource = 0;

        public int Id { get; }
        public string Name { get; }
        public int[] Pow { get; }
        public int EffectCount { get; }
        public TimeSpan EffectTimeSpan { get; }

        public PowDebuff(string name, int[] pow, int effectCount, TimeSpan effectTimeSpan)
        {
            Id             = idSource++;
            Name           = name;
            Pow            = pow;
            EffectCount    = effectCount;
            EffectTimeSpan = effectTimeSpan;
        }
    }
}
