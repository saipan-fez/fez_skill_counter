using FEZSkillCounter.Entity;
using FEZSkillUseCounter.Entity;
using SkillUseCounter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FEZSkillUseCounter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private SkillCountService _skillUseService = new SkillCountService();
        private List<SkillCount>  _skillList       = new List<SkillCount>();

        public MainWindow()
        {
            InitializeComponent();

            Loaded              += MainWindow_Loaded;
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            _skillUseService.SkillCountIncremented += _skillUseService_SkillCountIncremented; ;
            _skillUseService.SkillsUpdated         += _skillUseService_SkillsUpdated;
            _skillUseService.PowDebuffsUpdated     += _skillUseService_PowDebuffsUpdated;
            _skillUseService.PowUpdated            += _skillUseService_PowUpdated;
            _skillUseService.FpsUpdated            += _skillUseService_FpsUpdated;

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Logger.WriteLine(e.Exception.ToString());
            };

            UpdateSkillText();
            Logger.WriteLine("起動");
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _skillUseService.Start();
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState != MouseButtonState.Pressed)
            {
                return;
            }

            DragMove();
        }

        private void _skillUseService_FpsUpdated(object sender, double e)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                ProcessAvgTimeText.Text = e.ToString("F1");
            })));
        }

        private void _skillUseService_PowUpdated(object sender, int pow)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                PowText.Text = pow.ToString();
            })));
        }

        private void _skillUseService_PowDebuffsUpdated(object sender, PowDebuff[] e)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                DebuffText.Text = string.Join("\r\n", e.Select(x => x.Name));
            })));
        }

        private void _skillUseService_SkillsUpdated(object sender, Skill[] skills)
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

        private void _skillUseService_SkillCountIncremented(object sender, Skill skill)
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
    }
}
