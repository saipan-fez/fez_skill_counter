using FEZSkillCounter.Model;
using FEZSkillCounter.Model.Entity;
using FEZSkillCounter.Model.Repository;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SkillUseCounter;
using SkillUseCounter.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
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

    public class SkillCountUseCase : IDisposable
    {
        private static readonly string TxtFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "skillcount.txt");
        private static readonly string XmlFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "html\\skillcount.xml");
        private static readonly string NotifySoundFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets\\Sound\\book_notify.wav");

        public ReactivePropertySlim<string>               MapName                { get; }
        public ReactivePropertySlim<string>               WorkName               { get; }
        public ReactiveCollection<SkillCountDetailEntity> CurrentSkillCollection { get; }
        public ReactiveCollection<SkillCountEntity>       SkillCountHistories    { get; }
        public ReactivePropertySlim<WarEvents>            WarStatus              { get; }
        public ReactivePropertySlim<double>               AverageFps             { get; }
        public ReactivePropertySlim<double>               AttackKeepDamage       { get; }
        public ReactivePropertySlim<double>               DefenceKeepDamage      { get; }
        public ReactivePropertySlim<bool>                 IsBookUsing            { get; }
        public ReactivePropertySlim<int>                  Pow                    { get; }
        public ReactivePropertySlim<string>               PowDebuffs             { get; }
        public ReactiveProperty<bool>                     IsSkillCountFileSave   { get; }
        public ReactiveProperty<bool>                     IsDebugModeEnabled     { get; }

        private SkillCountRepository     _skillCountRepository;
        private SkillCountFileRepository _skillCountFileRepository;
        private SettingRepository        _settingRepository;
        private SkillCountService        _skillUseService;
        private BookUseNotificator       _bookUseNotificator;

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
            AttackKeepDamage       = new ReactivePropertySlim<double>(0d);
            DefenceKeepDamage      = new ReactivePropertySlim<double>(0d);
            IsBookUsing            = new ReactivePropertySlim<bool>(false);
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

        #region IDisposable 
        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (_skillUseService != null)
                {
                    _skillUseService.Dispose();
                    _skillUseService = null;
                }
            }
            finally
            {
                _disposed = true;
            }
        }
        #endregion

        public async Task SetUpAsync()
        {
            _bookUseNotificator = new BookUseNotificator(NotifySoundFilePath);

            _skillUseService = new SkillCountService();
            _skillUseService.WarStarted         += _skillUseService_WarStarted;
            _skillUseService.WarCanceled        += _skillUseService_WarCanceled;
            _skillUseService.WarFinished        += _skillUseService_WarFinished;
            _skillUseService.SkillUsed          += _skillUseService_SkillCountIncremented;
            _skillUseService.SkillsUpdated      += _skillUseService_SkillsUpdated;
            _skillUseService.PowDebuffsUpdated  += _skillUseService_PowDebuffsUpdated;
            _skillUseService.PowUpdated         += _skillUseService_PowUpdated;
            _skillUseService.ProcessTimeUpdated += _skillUseService_FpsUpdated;
            _skillUseService.KeepDamageUpdated  += _skillUseService_KeepDamageUpdated;
            _skillUseService.BookUsesUpdated    += _skillUseService_BookUsesUpdated;

            SkillCountHistories.AddRangeOnScheduler(_skillCountRepository.GetSkillCounts());

            _skillCountFileRepository = await SkillCountFileRepository.CreateAsync(TxtFilePath, XmlFilePath);
        }

        public void StartSkillCounter()
        {
            _skillUseService.Start();
        }

        public void StopSkillCounter()
        {
            _skillUseService.Stop();
        }

        public async Task ResetSkillCountAsync()
        {
            _skillUseService.Reset();
            CurrentSkillCollection.Clear();

            await UpdateSkillTextAsync();
        }

        public void CopySkillCountToClipboard(IEnumerable<SkillCountEntity> entities)
        {
            var entityStrings = entities.Select(x => 
                $"【マップ】{x.MapName}\r\n" +
                $"【スキル】\r\n" +
                $"{string.Join("\r\n", x.Details.Select(d => d.SkillShortName + "：" + d.Count))}");

            Clipboard.SetText(string.Join("\r\n\r\n", entityStrings));
        }

        private async void _skillUseService_WarStarted(object sender, WarStartedEventArgs e)
        {
            WarStatus.Value = WarEvents.WarStarted;
            MapName.Value   = e.Map.IsEmpty() ? string.Empty : e.Map.Name;

            // 戦争開始時にスキル回数をリセットする
            await ResetSkillCountAsync();
        }

        private void _skillUseService_WarCanceled(object sender, WarCanceledEventArgs e)
        {
            WarStatus.Value = WarEvents.WarCanceled;
            MapName.Value   = e.Map.IsEmpty() ? string.Empty : e.Map.Name;
        }

        private async void _skillUseService_WarFinished(object sender, WarFinishedEventArgs e)
        {
            WarStatus.Value = WarEvents.WarFinished;
            MapName.Value   = e.Map.IsEmpty() ? string.Empty : e.Map.Name;

            await SaveSkillCountAsync();
        }

        private void _skillUseService_FpsUpdated(object sender, ProcessTimeUpdatedEventArgs e)
        {
            AverageFps.Value = e.ProcessAvgTime;
        }

        private void _skillUseService_KeepDamageUpdated(object sender, KeepDamageUpdatedEventArgs e)
        {
            AttackKeepDamage.Value  = e.Damage.AttackKeepDamage;
            DefenceKeepDamage.Value = e.Damage.DefenceKeepDamage;

            // 書の使用通知のため状態をレポートする
            _bookUseNotificator.ReportCurrentStatusWithNotify(
                WarStatus.Value,
                IsBookUsing.Value,
                e.Damage);
        }

        private void _skillUseService_BookUsesUpdated(object sender, BookUsesUpdatedEventArgs e)
        {
            IsBookUsing.Value = e.IsBookUsed;
        }

        private void _skillUseService_PowUpdated(object sender, PowUpdatedEventArgs e)
        {
            Pow.Value = e.Pow;
        }

        private void _skillUseService_PowDebuffsUpdated(object sender, PowDebuffsUpdatedEventArgs e)
        {
            PowDebuffs.Value = e.PowDebuffs != null ?
                string.Join(Environment.NewLine, e.PowDebuffs.Select(x => x.Name)) :
                string.Empty;
        }

        private async void _skillUseService_SkillsUpdated(object sender, SkillsUpdatedEventArgs e)
        {
            var skills = e.Skills;
            var requireUpdate = false;
            if (skills != null && skills.Where(x => !x.IsEmpty()).Any())
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

            if (requireUpdate)
            {
                await UpdateSkillTextAsync();
            }
        }

        private async void _skillUseService_SkillCountIncremented(object sender, SkillUsedEventArgs e)
        {
            AddSkillIfNotExists(e.UsedSkill);

            var skillCount = CurrentSkillCollection.FirstOrDefault(x => x.SkillName == e.UsedSkill.Name);
            if (skillCount != null)
            {
                skillCount.Increment();

                await UpdateSkillTextAsync();
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
                SkillShortName = s.ShortName,
                WorkName       = s.WorkName,
            });

            return true;
        }

        private async Task UpdateSkillTextAsync()
        {
            if (IsSkillCountFileSave.Value)
            {
                await _skillCountFileRepository.SaveAsync(CurrentSkillCollection);
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
            SkillCountHistories.AddOnScheduler(entity);
        }
    }
}
