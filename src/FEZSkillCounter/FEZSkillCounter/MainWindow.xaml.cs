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

            Loaded += (_, __) => Task.Run(() => Proc());
            SkillUpdated += MainWindow_SkillUpdated;
            SkillCountIncremented += MainWindow_SkillCountIncremented;
            PowUpdated += MainWindow_MpUpdated;
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

        private void Proc()
        {
            while (true)
            {
                //Thread.Sleep(0);

                // TODO: 非戦争→戦争中に切り替わったら使用回数をクリア

                var start = _stopwatch.ElapsedMilliseconds;

                using (var bitmap = _screenShooter.Shoot())
                {
                    // 解析可能か確認
                    if (!CanAnalyze(bitmap))
                    {
                        continue;
                    }

                    // TODO: Pow取得とスキル取得を並列化
                    // 現在のPowを取得
                    var previousPow = _currentPow;
                    _currentPow = GetPow(bitmap);

                    if (_currentPow == int.MaxValue)
                    {
                        // 取得失敗していれば終了
                        continue;
                    }



                    // 前回のPowとの差分取得
                    var powDiff = 0;
                    if (previousPow != int.MaxValue)
                    {
                        powDiff = _currentPow - previousPow;

                        // TODO: 回復とデバフを考慮

                        // 考慮外
                        //   回復中(Powゲージが増加中)の


                        PowUpdated?.BeginInvoke(
                            this,
                            _currentPow,
                            null,
                            null);
                    }

                    // スキル取得
                    var previousSkills = _currentSkills;
                    _currentSkills = GetSkills(bitmap);

                    // スキルが更新されていれば通知
                    if (previousSkills == null ||
                        _currentSkills.SequenceEqual(previousSkills, x => x.Name))
                    {
                        SkillUpdated?.BeginInvoke(
                            this,
                            _currentSkills,
                            null,
                            null);
                    }

                    // 選択中スキルを取得
                    var activeSkill = _currentSkills.FirstOrDefault(x => x.IsActive);

                    if (powDiff != 0)
                    {
                        Debug.WriteLine(powDiff);
                    }

                    // Powの差分が選択中のスキルと一致していれば更新通知
                    if (activeSkill.Pow.Any(x => -x == powDiff))
                    {
                        SkillCountIncremented?.BeginInvoke(
                            this,
                            activeSkill,
                            null,
                            null);
                    }
                }

                var end = _stopwatch.ElapsedMilliseconds;
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

            //Debug.WriteLine("Pow:" + pow);

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

        //private List<PowBuff> GetActivePowBuff()
        //{
        //    // 現在のPowに影響を与えるバフ一覧を取得

        //    return new List<PowBuff>();
        //}

        private List<PowDebuff> GetActivePowDebuff()
        {
            // TODO: 現在のPowに影響を与えるデバフ一覧を取得
            var powDebufs = new List<PowDebuff>();




            return new List<PowDebuff>();
        }

        //private List<PowBuff> PowBuffList = new List<PowBuff>()
        //{
        //    // PW回復
        //    //    5: ライトパワーポッド系
        //    //   10: パワーポッド系
        //    //   15: ハイパワーポッド系
        //    new PowBuff("Pow回復", new int[]{ 5, 10, 15 }, TimeSpan.FromSeconds(4)),

        //    // ヴィガーエイド
        //    new PowBuff("ヴィガーエイド", new int[]{ 5 }, TimeSpan.FromSeconds(1)),

        //    // HP/Pow回復アイテムではPow6回復のアイテム(ラバーズメディスン/ミクカラージュース)があるが、
        //    // イベントアイテムでほぼ使われることが無いため一覧からは除外する
        //    // (間違えたスキル使用判定を避けるため)
        //    //new PowBuff("HP/Pow回復", new int[] { 6 }, TimeSpan.FromSeconds(4)),
        //};

        private List<PowDebuff> PowDebuffList = new List<PowDebuff>()
        {
            // パワーブレイク
            //   Lv1-3で減少Powが異なる
            new PowDebuff("パワーブレイク", new int[]{ -15, -20, -25 }, 8, TimeSpan.FromSeconds(3)),
        };
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

    //public class PowBuff
    //{
    //    public string Name { get; }
    //    public int[] Pow { get; }
    //    public TimeSpan EffectTimeSpan { get; }

    //    // 回復は上書き可能で回数判定ができないため使用しない
    //    //public int EffectCount { get; set; }

    //    public PowBuff(string name, int[] pow, TimeSpan effectTimeSpan)
    //    {
    //        Name           = name;
    //        Pow            = pow;
    //        EffectTimeSpan = effectTimeSpan;
    //    }
    //}

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
