using FEZSkillCounter.Model.Entity;
using FEZSkillCounter.Model.Repository;
using MahApps.Metro.Controls;
using SkillUseCounter;
using SkillUseCounter.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace FEZSkillCounter.View
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.FirstChanceException += (s, e) =>
            {
                Logger.WriteException(e.Exception);
            };

            _currentSkillCount     = new SkillCountEntity();
            _skillCountHistoryList = new ObservableCollection<SkillCountEntity>();
            SkillCountDataGrid.ItemsSource = _currentSkillCount.Details;

            Loaded                              += MainWindow_Loaded;
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
            _skillUseService = new SkillCountService();
            _skillUseService.Start();
            _skillCountRepository = SkillCountRepository.Create(DbFilePath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var d in _currentSkillCount.Details)
            {
                d.Count = 0;
            }

            UpdateSkillText();
        }

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

                SaveSkillCount();
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
                sw.Flush();
            }
        }

        private void SaveSkillCount(string mapName, IEnumerable<SkillCount> skillCounts)
        {
            if (!skillCounts.Any())
            {
                return;
            }

            var entity = new SkillCountEntity()
            {
                RecordDate = DateTime.Now,
                MapName    = mapName,
                WorkName   = skillCounts.First(x => x.s).
            };

            _skillCountRepository.Add(
        }
    }
}
