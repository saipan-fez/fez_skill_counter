using FEZSkillCounter.Model.Entity;
using FEZSkillCounter.UseCase;
using MaterialDesignThemes.Wpf;
using Reactive.Bindings;
using Reactive.Bindings.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace FEZSkillCounter.ViewModel
{
    public class MainWindowViewModel
    {
        private SkillCountUseCase _skillCountUseCase;

        public ReactivePropertySlim<bool>              IsLoaded             { get; }
        public ReadOnlyReactivePropertySlim<string>    MapName              { get; }
        public ReadOnlyReactivePropertySlim<string>    WorkName             { get; }
        public ReadOnlyReactivePropertySlim<WarEvents> WarStatus            { get; }
        public ReadOnlyReactivePropertySlim<double>    AverageFps           { get; }
        public ReadOnlyReactivePropertySlim<int>       Pow                  { get; }
        public ReadOnlyReactivePropertySlim<string>    PowDebuffs           { get; }
        public ReactiveProperty<bool>                  IsSkillCountFileSave { get; }
        public ReactiveProperty<bool>                  IsDebugModeEnabled   { get; }

        public ReactiveProperty<bool> IsWarriorFilter  { get; }
        public ReactiveProperty<bool> IsSorcererFilter { get; }
        public ReactiveProperty<bool> IsScoutFilter    { get; }
        public ReactiveProperty<bool> IsCestusFilter   { get; }
        public ReactiveProperty<bool> IsFencerFilter   { get; }

        public ReadOnlyReactiveCollection<SkillCountDetailEntity>         CurrentSkillCollection    { get; }
        public IFilteredReadOnlyObservableCollection<SkillCountEntity>    SkillCountHistories       { get; }
        public ReactiveProperty<SkillCountEntity>                         SelectedSkillCountHistory { get; }
        public ReadOnlyReactivePropertySlim<List<SkillCountDetailEntity>> SelectedSkillCountDatails { get; }

        public ReactiveCommand LoadedCommand { get; }
        public ReactiveCommand ClosedCommand { get; }
        public ReactiveCommand ResetCommand  { get; }

        public ReactiveCommand<object> CopySkillCountsCommand { get; }

        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();

        public MainWindowViewModel()
        {
            _skillCountUseCase = new SkillCountUseCase();

            IsLoaded     = new ReactivePropertySlim<bool>(false);
            MapName = _skillCountUseCase.MapName
                .Select(x => string.IsNullOrEmpty(x) ? "" : x)
                .ToReadOnlyReactivePropertySlim();
            WorkName = _skillCountUseCase.WorkName
                .Select(x => string.IsNullOrEmpty(x) ? "" : x)
                .ToReadOnlyReactivePropertySlim();
            WarStatus = _skillCountUseCase.WarStatus.ToReadOnlyReactivePropertySlim();
            AverageFps = _skillCountUseCase.AverageFps.ToReadOnlyReactivePropertySlim();
            Pow = _skillCountUseCase.Pow.ToReadOnlyReactivePropertySlim();
            PowDebuffs = _skillCountUseCase.PowDebuffs
                .Select(x => string.IsNullOrEmpty(x) ? "" : x)
                .ToReadOnlyReactivePropertySlim();

            IsSkillCountFileSave = _skillCountUseCase.IsSkillCountFileSave.ToReactiveProperty();
            IsSkillCountFileSave.Subscribe(x =>
            {
                _skillCountUseCase.IsSkillCountFileSave.Value = x;
            });
            IsDebugModeEnabled   = _skillCountUseCase.IsDebugModeEnabled.ToReactiveProperty();
            IsDebugModeEnabled.Subscribe(x =>
            {
                _skillCountUseCase.IsDebugModeEnabled.Value = x;
            });

            IsWarriorFilter  = new ReactiveProperty<bool>(true);
            IsSorcererFilter = new ReactiveProperty<bool>(true);
            IsScoutFilter    = new ReactiveProperty<bool>(true);
            IsCestusFilter   = new ReactiveProperty<bool>(true);
            IsFencerFilter   = new ReactiveProperty<bool>(true);
            void doFilter(bool _)
            {
                var filterWorks = new List<string>();
                if (IsWarriorFilter.Value) filterWorks.Add("ウォーリアー");
                if (IsSorcererFilter.Value) filterWorks.Add("ソーサラー");
                if (IsScoutFilter.Value) filterWorks.Add("スカウト");
                if (IsCestusFilter.Value) filterWorks.Add("セスタス");
                if (IsFencerFilter.Value) filterWorks.Add("フェンサー");

                SkillCountHistories?.Refresh(entity => filterWorks.Contains(entity.WorkName));
            }
            IsWarriorFilter.Subscribe(doFilter);
            IsSorcererFilter.Subscribe(doFilter);
            IsScoutFilter.Subscribe(doFilter);
            IsCestusFilter.Subscribe(doFilter);
            IsFencerFilter.Subscribe(doFilter);

            CurrentSkillCollection = _skillCountUseCase.CurrentSkillCollection.ToReadOnlyReactiveCollection();
            SkillCountHistories = _skillCountUseCase.SkillCountHistories.ToFilteredReadOnlyObservableCollection(x =>
            {
                var filterWorks = new List<string>();
                if (IsWarriorFilter.Value) filterWorks.Add("ウォーリアー");
                if (IsSorcererFilter.Value) filterWorks.Add("ソーサラー");
                if (IsScoutFilter.Value) filterWorks.Add("スカウト");
                if (IsCestusFilter.Value) filterWorks.Add("セスタス");
                if (IsFencerFilter.Value) filterWorks.Add("フェンサー");

                return filterWorks.Contains(x.WorkName);
            });
            SelectedSkillCountHistory = new ReactiveProperty<SkillCountEntity>();
            SelectedSkillCountDatails = SelectedSkillCountHistory
                .Select(x => x != null ? x.Details : new List<SkillCountDetailEntity>())
                .ToReadOnlyReactivePropertySlim();

            LoadedCommand = new ReactiveCommand();
            LoadedCommand.Subscribe(async () =>
            {
                await _skillCountUseCase.SetUpAsync();
                _skillCountUseCase.StartSkillCounter();

                IsLoaded.Value = true;
            });

            ClosedCommand = new ReactiveCommand();
            ClosedCommand.Subscribe(() =>
            {
                _skillCountUseCase.StopSkillCounter();

                IsLoaded.Value = false;
            });

            ResetCommand = new ReactiveCommand();
            ResetCommand.Subscribe(() =>
            {
                _skillCountUseCase.ResetSkillCount();
            });

            CopySkillCountsCommand = SelectedSkillCountHistory
                .Select(x => x != null)
                .ToReactiveCommand();
            CopySkillCountsCommand.Subscribe(o =>
            {
                var entities = ((System.Collections.IList)o).Cast<SkillCountEntity>();
                _skillCountUseCase.CopySkillCountToClipboard(entities);

                MessageQueue.Enqueue("クリップボードにコピーしました");
            });
        }
    }

    public enum FilterWork
    {
        すべて,
        ウォーリアー,
        ソーサラー,
        スカウト,
        フェンサー,
        セスタス
    }
}
