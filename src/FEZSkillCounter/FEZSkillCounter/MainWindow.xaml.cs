using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using FEZSkillCounter.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

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

            Loaded += (_, __) => Task.Run(() => Run());
            SkillUpdated += MainWindow_SkillUpdated;
            SkillCountIncremented += MainWindow_SkillCountIncremented;
            PowDebuffUpdated += MainWindow_PowDebuffUpdated;
            PowUpdated += MainWindow_MpUpdated;

            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Logger.WriteLine(e.Exception.ToString());
            };

            UpdateSkillText();
            Logger.WriteLine("起動");
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed)
            {
                return;
            }
  
            DragMove(); 
        }

        private void MainWindow_MpUpdated(object sender, int pow)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                PowText.Text = pow.ToString();
            })));
        }

        private void MainWindow_PowDebuffUpdated(object sender, PowDebuff[] e)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                DebuffText.Text = string.Join("\r\n", e.Select(x => x.Name));
            })));
        }

        private List<SkillCount> _skillList = new List<SkillCount>();

        private void MainWindow_SkillUpdated(object sender, Skill[] skills)
        {
            bool requireUpdate = false;
            foreach (var s in skills.Where(x => !x.IsEmpty()))
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
            var text = string.Join(Environment.NewLine, _skillList.Select(x => x.Skill.Name + "：" + x.Count));

            Dispatcher.BeginInvoke(((Action)(() =>
            {
                SkillText.Text = text;

                using (var sw = new StreamWriter("skillcount.txt", false, Encoding.UTF8))
                {
                    sw.WriteLine(text);
                }
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

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach(var b in list.Select((i, x) => new { obj = i, idx = x }))
            {
                b.obj.Save($"{b.idx.ToString()}.bmp");
            }
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            lock(list)
            {
                list.Clear();
            }

            _skillList.Clear();
            UpdateSkillText();
        }

        private List<Bitmap> list = new List<Bitmap>();

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
        private SkillUseAlgorithm _skillCountAlgorithm = new SkillUseAlgorithm();

        private Status _status = Status.Waiting;

        private int _previousPow = int.MaxValue;
        private Skill[] _previousSkills = null;
        private PowDebuff[] _previousPowDebuff = null;

        private Stopwatch _stopwatch = Stopwatch.StartNew();
        private long processTotalTime  = 0;
        private long processTotalCount = 0;

        private event EventHandler<int> PowUpdated;
        private event EventHandler<Skill[]> SkillUpdated;
        private event EventHandler<PowDebuff[]> PowDebuffUpdated;
        private event EventHandler<Skill> SkillCountIncremented;

        private void Run()
        {
            while (true)
            {
                var start = _stopwatch.ElapsedMilliseconds;
                var result = false;
                try
                {
                    result = Analyze();
                }
                catch
                {
                    // nop
                }
                finally
                {
                    var end = _stopwatch.ElapsedMilliseconds;

                    if (result)
                    {
                        processTotalTime += (end - start);
                        processTotalCount++;

                        if (processTotalCount % 30 == 0)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                ProcessAvgTimeText.Text = ((double)processTotalTime / processTotalCount).ToString("F1");
                            });
                        }
                    }

                    // 処理が失敗している場合は、即終了している。
                    // ループによるCPU使用率を抑えるためにwait
                    if (!result)
                    {
                        Thread.Sleep(30);
                    }
                }
            }
        }

        private bool Analyze()
        {
            // TODO: 非戦争→戦争中に切り替わったら使用回数をクリア

            using (var screenShot = _screenShooter.Shoot())
            {
                // 解析可能か確認
                if (!CanAnalyze(screenShot.Image))
                {
                    return false;
                }

                // Powを取得
                var pow = GetPow(screenShot.Image);
                if (pow != int.MaxValue)
                {
                    // 前回から更新されていれば通知
                    if (_previousPow == int.MaxValue || _previousPow != pow)
                    {
                        PowUpdated?.BeginInvoke(this, pow, null, null);
                        _previousPow = pow;
                    }
                }
                else
                {
                    if (list.Count < 50)
                    {
                    lock (list)
                    {
                        list.Add(screenShot.Image.Clone(
                            new Rectangle(0, 0, screenShot.Image.Width, screenShot.Image.Height),
                            PixelFormat.Format32bppRgb));
                        Logger.WriteLine("Pow detect errored. cnt:" + list.Count);
                    }
                    }

                    // 失敗していれば前回の値をそのまま使う
                    pow = _previousPow;
                }

                // スキル取得
                var skills = GetSkills(screenShot.Image);
                if (skills != null)
                {
                    // 前回から更新されていれば通知
                    if (_previousSkills == null || !_previousSkills.SequenceEqual(skills, x => x.Name))
                    {
                        SkillUpdated?.BeginInvoke(this, skills, null, null);
                        _previousSkills = skills;
                    }
                }
                else
                {
                    // 失敗していれば前回の値をそのまま使う
                    skills = _previousSkills;
                }

                // デバフ取得
                var powDebuff = GetPowDebuffs(screenShot.Image);
                if (powDebuff != null)
                {
                    // 前回から更新されていれば通知
                    if (_previousPowDebuff == null || !_previousPowDebuff.SequenceEqual(powDebuff, x => x.Name))
                    {
                        PowDebuffUpdated?.BeginInvoke(this, powDebuff, null, null);
                        _previousPowDebuff = powDebuff;
                    }
                }
                else
                {
                    // 失敗していれば前回の値をそのまま使う
                    powDebuff = _previousPowDebuff;
                }

                // 選択中スキルを取得
                var activeSkill = skills.FirstOrDefault(x => x.IsActive);

                // 取得失敗していれば終了
                if (pow == int.MaxValue || skills == null || powDebuff == null || activeSkill == null)
                {
                    return false;
                }

                // スキルを使ったかどうかチェック
                var isSkillUsed = _skillCountAlgorithm.IsSkillUsed(
                                screenShot.TimeStamp,
                                pow,
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

            return true;
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
                else
                {
                    Logger.WriteLine($"Pow detect error. houndred:{houndreds} tens:{tens} ones:{ones}");
                }
            }

            return pow;
        }

        private int GetPowNum(Bitmap bitmap)
        {
            var hash = bitmap.FillPaddingToZero().SHA1Hash();
            if (PowStorage.Table.TryGetValue(hash, out int pow))
            {
                return pow;
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
        private readonly RGBColor PowerBreakCmpColor1 = new RGBColor (Color.FromArgb(30, 125, 183));
        private readonly RGBColor PowerBreakCmpColor2 = new RGBColor (Color.FromArgb(49, 134, 187));

        private double ColorDiffThreashold = 10.0d;

        // パワーブレイク
        //   Lv1-3で減少Powが異なる
        private readonly PowDebuff PowerBreak = 
            new PowDebuff("パワーブレイク", new int[]{ -15, -20, -25 }, 8, TimeSpan.FromSeconds(3));

        private PowDebuff[] GetPowDebuffs(Bitmap bitmap)
        {
            var converter = new ColourfulConverter(){ WhitePoint = Illuminants.D65 };
            var powerBreakCmpLabColor1 = converter.ToLab(PowerBreakCmpColor1);
            var powerBreakCmpLabColor2 = converter.ToLab(PowerBreakCmpColor2);
            var difference = new CIE76ColorDifference();

            for (int i = 0; i < MaxPowerDebuffCount; i++)
            {
                var x1 = bitmap.Width  - 28 - (32 * i);
                var x2 = bitmap.Width  - 34 - (32 * i);
                var y  = bitmap.Height - 20;

                var c1 = converter.ToLab(new RGBColor(bitmap.GetPixel(x1, y)));
                var c2 = converter.ToLab(new RGBColor(bitmap.GetPixel(x2, y)));

                var diff1 = difference.ComputeDifference(powerBreakCmpLabColor1, c1);
                var diff2 = difference.ComputeDifference(powerBreakCmpLabColor2, c2);

                if (Math.Abs(diff1) < ColorDiffThreashold &&
                    Math.Abs(diff2) < ColorDiffThreashold)
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
        private const string UnknownSkillName = "Unknown";

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

        public static Skill Empty
        {
            get
            {
                return new Skill("Unknown", new int[] { int.MaxValue }, false);
            }
        }

        public bool IsEmpty()
        {
            return Name == UnknownSkillName;
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
