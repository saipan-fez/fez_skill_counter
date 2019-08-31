using FEZSkillCounter.Model.Entity;
using FEZSkillCounter.UseCase;
using Reactive.Bindings;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace FEZSkillCounter.ViewModel
{
    public class MainWindowViewModel
    {
        private SkillCountUseCase _skillCountUseCase;

        public ReactivePropertySlim<bool>              IsLoaded   { get; }
        public ReadOnlyReactivePropertySlim<string>    MapName    { get; }
        public ReadOnlyReactivePropertySlim<string>    WorkName   { get; }
        public ReadOnlyReactivePropertySlim<WarEvents> WarStatus  { get; }
        public ReadOnlyReactivePropertySlim<double>    AverageFps { get; }
        public ReadOnlyReactivePropertySlim<int>       Pow        { get; }
        public ReadOnlyReactivePropertySlim<string>    PowDebuffs { get; }

        public ReadOnlyReactiveCollection<SkillCountDetailEntity>         CurrentSkillCollection    { get; }
        public ReadOnlyReactiveCollection<SkillCountEntity>               SkillCountHistories       { get; }
        public ReactiveProperty<SkillCountEntity>                         SelectedSkillCountHistory { get; }
        public ReadOnlyReactivePropertySlim<List<SkillCountDetailEntity>> SelectedSkillCountDatails { get; }

        public ReactiveCommand LoadedCommand { get; }
        public ReactiveCommand ClosedCommand { get; }
        public ReactiveCommand ResetCommand  { get; }

        public MainWindowViewModel()
        {
            _skillCountUseCase = new SkillCountUseCase();

            IsLoaded   = new ReactivePropertySlim<bool>(false);
            MapName    = _skillCountUseCase.MapName
                .Select(x => string.IsNullOrEmpty(x) ? "" : x)
                .ToReadOnlyReactivePropertySlim();
            WorkName   = _skillCountUseCase.WorkName
                .Select(x => string.IsNullOrEmpty(x) ? "" : x)
                .ToReadOnlyReactivePropertySlim();
            WarStatus  = _skillCountUseCase.WarStatus.ToReadOnlyReactivePropertySlim();
            AverageFps = _skillCountUseCase.AverageFps.ToReadOnlyReactivePropertySlim();
            Pow        = _skillCountUseCase.Pow.ToReadOnlyReactivePropertySlim();
            PowDebuffs = _skillCountUseCase.PowDebuffs
                .Select(x => string.IsNullOrEmpty(x) ? "" : x)
                .ToReadOnlyReactivePropertySlim();

            CurrentSkillCollection    = _skillCountUseCase.CurrentSkillCollection.ToReadOnlyReactiveCollection();
            SkillCountHistories       = _skillCountUseCase.SkillCountHistories.ToReadOnlyReactiveCollection();
            SelectedSkillCountHistory = new ReactiveProperty<SkillCountEntity>();
            SelectedSkillCountDatails = SelectedSkillCountHistory
                .Where(x => x != null)
                .Select(x => x.Details)
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
        }
    }
}
