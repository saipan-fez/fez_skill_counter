using FEZSkillCounter.Entity;
using MahApps.Metro.Controls;
using RepositoryService;
using SkillUseCounter;
using SkillUseCounter.Entity;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace FEZSkillCounter
{
    public partial class MainWindow : MetroWindow
    {
        private SkillCountService                 _skillUseService = new SkillCountService();
        private ObservableCollection<SkillCount>  _skillList       = new ObservableCollection<SkillCount>();
        private SkillCountRepository _skillCountRepository;

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Logger.WriteException(e.Exception);
            };

            SkillCountDataGrid.ItemsSource = _skillList;

            Loaded              += MainWindow_Loaded;
            //MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            _skillUseService.WarStarted         += _skillUseService_WarStarted;
            _skillUseService.WarCanceled        += _skillUseService_WarCanceled;
            _skillUseService.WarFinished        += _skillUseService_WarFinished;
            _skillUseService.SkillUsed          += _skillUseService_SkillCountIncremented;
            _skillUseService.SkillsUpdated      += _skillUseService_SkillsUpdated;
            _skillUseService.PowDebuffsUpdated  += _skillUseService_PowDebuffsUpdated;
            _skillUseService.PowUpdated         += _skillUseService_PowUpdated;
            _skillUseService.ProcessTimeUpdated += _skillUseService_FpsUpdated;

            UpdateSkillText();

            Logger.WriteLine("FEZSkillCounter started.");
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _skillUseService.Start();

            // TODO: ファイルパス
            _skillCountRepository = SkillCountRepository.Create("");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var s in _skillList)
            {
                s.Reset();
            }

            UpdateSkillText();
        }

        //private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ButtonState != MouseButtonState.Pressed)
        //    {
        //        return;
        //    }

        //    DragMove();
        //}

        private void _skillUseService_WarStarted(object sender, Map e)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                StatusText.Text = "戦争中";
                MapText.Text    = e.IsEmpty() ? "unknown" : e.Name;
            })));
        }

        private void _skillUseService_WarCanceled(object sender, Map e)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                StatusText.Text = "戦争キャンセル";
                MapText.Text    = e.IsEmpty() ? "unknown" : e.Name;
            })));
        }

        private void _skillUseService_WarFinished(object sender, Map e)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                StatusText.Text = "戦争終了";
                MapText.Text    = e.IsEmpty() ? "unknown" : e.Name;
            })));
        }

        private void _skillUseService_FpsUpdated(object sender, double e)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                ProcessAvgTimeText.Text = e.ToString("F1") + "ms";
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
                DebuffText.Text = e != null ? string.Join("\r\n", e.Select(x => x.Name)) : "";
            })));
        }

        private void _skillUseService_SkillsUpdated(object sender, Skill[] skills)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                bool requireUpdate = false;
                if (skills != null)
                {
                    foreach (var s in skills.Where(x => !x.IsEmpty()))
                    {
                        if (!_skillList.Any(x => x.Name == s.Name))
                        {
                            _skillList.Add(new SkillCount(s));
                            requireUpdate = true;
                        }
                    }
                }
                else
                {
                    _skillList.Clear();
                    requireUpdate = true;
                }

                if (requireUpdate)
                {
                    UpdateSkillText();
                }
            })));
        }

        private void _skillUseService_SkillCountIncremented(object sender, Skill skill)
        {
            Dispatcher.BeginInvoke(((Action)(() =>
            {
                var skillCount = _skillList.FirstOrDefault(x => x.Name == skill.Name);
                if (skillCount != null)
                {
                    skillCount.Increment();

                    UpdateSkillText();
                }
            })));
        }

        private void UpdateSkillText()
        {
            var text = string.Join(Environment.NewLine, _skillList.Select(x => x.ShortName + "：" + x.Count));

            SkillText.Text = text;

            using (var sw = new StreamWriter("skillcount.txt", false, Encoding.UTF8))
            {
                sw.WriteLine(text);
            }
        }
    }
}
