﻿using FEZSkillCounter.Model.Entity;
using FEZSkillCounter.Model.Repository;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SkillUseCounter;
using SkillUseCounter.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace FEZSkillCounter.UseCase
{
    public enum WarEvents
    {
        Invalid,
        WarStarted,
        WarCanceled,
        WarFinished,
    }

    public class SkillCountUseCase
    {
        private static readonly string TxtFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "skillcount.txt");

        public ReactivePropertySlim<string>               MapName                { get; }
        public ReactivePropertySlim<string>               WorkName               { get; }
        public ReactiveCollection<SkillCountDetailEntity> CurrentSkillCollection { get; }
        public ReactiveCollection<SkillCountEntity>       SkillCountHistories    { get; }
        public ReactivePropertySlim<WarEvents>            WarStatus              { get; }
        public ReactivePropertySlim<double>               AverageFps             { get; }
        public ReactivePropertySlim<int>                  Pow                    { get; }
        public ReactivePropertySlim<string>               PowDebuffs             { get; }
        public ReactiveProperty<bool>                     IsSkillCountFileSave   { get; }
        public ReactiveProperty<bool>                     IsDebugModeEnabled     { get; }

        private SkillCountRepository     _skillCountRepository;
        private SkillCountFileRepository _skillCountFileRepository;
        private SettingRepository        _settingRepository;
        private SkillCountService        _skillUseService;

        public SkillCountUseCase()
        {
            _skillCountRepository = new SkillCountRepository(Application.Current.GetAppDb());
            _settingRepository    = new SettingRepository(Application.Current.GetAppDb());

            var setting = _settingRepository.GetSetting();

            MapName                = new ReactivePropertySlim<string>(string.Empty);
            WorkName               = new ReactivePropertySlim<string>(string.Empty);
            CurrentSkillCollection = new ReactiveCollection<SkillCountDetailEntity>();
            SkillCountHistories    = new ReactiveCollection<SkillCountEntity>();
            WarStatus              = new ReactivePropertySlim<WarEvents>(WarEvents.Invalid);
            AverageFps             = new ReactivePropertySlim<double>(0d);
            Pow                    = new ReactivePropertySlim<int>(0);
            PowDebuffs             = new ReactivePropertySlim<string>(string.Empty);
            IsSkillCountFileSave   = setting.ObserveProperty(x => x.IsSkillCountFileSave).ToReactiveProperty();
            IsDebugModeEnabled     = setting.ObserveProperty(x => x.IsDebugModeEnabled).ToReactiveProperty();

            IsSkillCountFileSave.Subscribe(async x =>
            {
                setting.IsSkillCountFileSave = x;
                await _settingRepository.UpdateAsync();
            });
            IsDebugModeEnabled.Subscribe(async x =>
            {
                // ログ出力を切り替え
                Logger.IsLogFileOutEnabled = x;

                setting.IsDebugModeEnabled = x;
                await _settingRepository.UpdateAsync();
            });
        }

        public async Task SetUpAsync()
        {
            _skillUseService = new SkillCountService();
            _skillUseService.WarStarted         += _skillUseService_WarStarted;
            _skillUseService.WarCanceled        += _skillUseService_WarCanceled;
            _skillUseService.WarFinished        += _skillUseService_WarFinished;
            _skillUseService.SkillUsed          += _skillUseService_SkillCountIncremented;
            _skillUseService.SkillsUpdated      += _skillUseService_SkillsUpdated;
            _skillUseService.PowDebuffsUpdated  += _skillUseService_PowDebuffsUpdated;
            _skillUseService.PowUpdated         += _skillUseService_PowUpdated;
            _skillUseService.ProcessTimeUpdated += _skillUseService_FpsUpdated;

            SkillCountHistories.AddRangeOnScheduler(_skillCountRepository.GetSkillCounts());

            _skillCountFileRepository = await SkillCountFileRepository.CreateAsync(TxtFilePath);
        }

        public void StartSkillCounter()
        {
            _skillUseService.Start();
        }

        public void StopSkillCounter()
        {
            _skillUseService.Stop();
        }

        public void ResetSkillCount()
        {
            _skillUseService.Reset();
            CurrentSkillCollection.Clear();

            UpdateSkillText();
        }

        public void CopySkillCountToClipboard(IEnumerable<SkillCountEntity> entities)
        {
            var entityStrings = entities.Select(x => 
                $"【マップ】{x.MapName}\r\n" +
                $"【スキル】\r\n" +
                $"{string.Join("\r\n", x.Details.Select(d => d.SkillShortName + "：" + d.Count))}");

            Clipboard.SetText(string.Join("\r\n\r\n", entityStrings));
        }

        private void _skillUseService_WarStarted(object sender, Map e)
        {
            WarStatus.Value = WarEvents.WarStarted;
            MapName.Value   = e.IsEmpty() ? string.Empty : e.Name;
        }

        private void _skillUseService_WarCanceled(object sender, Map e)
        {
            WarStatus.Value = WarEvents.WarCanceled;
            MapName.Value   = e.IsEmpty() ? string.Empty : e.Name;
        }

        private async void _skillUseService_WarFinished(object sender, Map e)
        {
            WarStatus.Value = WarEvents.WarFinished;
            MapName.Value   = e.IsEmpty() ? string.Empty : e.Name;

            await SaveSkillCountAsync();
        }

        private void _skillUseService_FpsUpdated(object sender, double e)
        {
            AverageFps.Value = e;
        }

        private void _skillUseService_PowUpdated(object sender, int pow)
        {
            Pow.Value = pow;
        }

        private void _skillUseService_PowDebuffsUpdated(object sender, PowDebuff[] e)
        {
            PowDebuffs.Value = e != null ?
                string.Join(Environment.NewLine, e.Select(x => x.Name)) :
                string.Empty;
        }

        private void _skillUseService_SkillsUpdated(object sender, Skill[] skills)
        {
            var requireUpdate = false;
            if (skills != null)
            {
                // 職が変更されていればスキル一覧をクリア
                if (skills.Where(x => !x.IsEmpty()).FirstOrDefault().WorkName != WorkName.Value)
                {
                    CurrentSkillCollection.Clear();
                }

                foreach (var s in skills.Where(x => !x.IsEmpty()))
                {
                    requireUpdate = AddSkillIfNotExists(s);
                }
            }
            else
            {
                CurrentSkillCollection.Clear();
                requireUpdate = true;
            }

            if (requireUpdate)
            {
                UpdateSkillText();
            }
        }

        private void _skillUseService_SkillCountIncremented(object sender, Skill skill)
        {
            AddSkillIfNotExists(skill);

            var skillCount = CurrentSkillCollection.FirstOrDefault(x => x.SkillName == skill.Name);
            if (skillCount != null)
            {
                skillCount.Increment();

                UpdateSkillText();
            }
        }

        private bool AddSkillIfNotExists(Skill s)
        {
            if (CurrentSkillCollection.Any(x => x.SkillName == s.Name))
            {
                return false;
            }

            if (!s.IsEmpty())
            {
                WorkName.Value = s.WorkName;
            }

            CurrentSkillCollection.Add(new SkillCountDetailEntity()
            {
                SkillName      = s.Name,
                SkillShortName = s.ShortName
            });

            return true;
        }

        private void UpdateSkillText()
        {
            if (IsSkillCountFileSave.Value)
            {
                _skillCountFileRepository.Save(CurrentSkillCollection);
            }
        }

        private async Task SaveSkillCountAsync()
        {
            if (!CurrentSkillCollection.Any())
            {
                return;
            }

            var entity = new SkillCountEntity()
            {
                RecordDate = DateTime.Now,
                MapName    = MapName.Value,
                WorkName   = WorkName.Value,
                Details    = CurrentSkillCollection.ToList()
            };

            await _skillCountRepository.SaveAsync(entity);

            // 履歴に追加
            SkillCountHistories.Add(entity);
        }
    }
}