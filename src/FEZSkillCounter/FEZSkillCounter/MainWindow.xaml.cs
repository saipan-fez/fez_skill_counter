using FEZSkillCounter.Algorithm;
using FEZSkillCounter.Entity;
using FEZSkillCounter.Recognizer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
            SkillCountIncremented += MainWindow_SkillCountIncremented;
            _skillArrayRecognizer.Updated += MainWindow_SkillUpdated;
            _powDebuffArrayRecognizer.Updated += MainWindow_PowDebuffUpdated;
            _powRecognizer.Updated += MainWindow_MpUpdated;

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

        private PreRecognizer            _preRecognizer            = new PreRecognizer();
        private WarStateRecognizer       _warStateRecognizer       = new WarStateRecognizer();
        private SkillArrayRecognizer     _skillArrayRecognizer     = new SkillArrayRecognizer();
        private PowDebuffArrayRecognizer _powDebuffArrayRecognizer = new PowDebuffArrayRecognizer();
        private PowRecognizer            _powRecognizer            = new PowRecognizer();

        private FEZScreenShotStorage _screenShotStorage = new FEZScreenShotStorage();
        private SkillUseAlgorithm _skillCountAlgorithm  = new SkillUseAlgorithm();

        private Stopwatch _stopwatch = Stopwatch.StartNew();
        private long processTotalTime  = 0;
        private long processTotalCount = 0;

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
