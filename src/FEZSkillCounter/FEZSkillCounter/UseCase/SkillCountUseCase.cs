using FEZSkillCounter.Model.Entity;
using FEZSkillCounter.Model.Repository;
using Reactive.Bindings;
using SkillUseCounter;
using SkillUseCounter.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        private static readonly string DbFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "skillcount.db");

        public ReactivePropertySlim<SkillCountEntity> CurrentSkillCount { get; }
        public ReactiveCollection<SkillCountEntity>   SkillCountHitory  { get; }
        public ReactivePropertySlim<WarEvents>        WarStatus         { get; }
        public ReactivePropertySlim<double>           AverageFps        { get; }
        public ReactivePropertySlim<int>              Pow               { get; }
        public ReactivePropertySlim<string>           PowDebuffs        { get; }

        private SkillCountRepository _skillCountRepository;
        private SkillCountService    _skillUseService;

        private SkillCountUseCase()
        {
            CurrentSkillCount = new ReactivePropertySlim<SkillCountEntity>(new SkillCountEntity());
            SkillCountHitory  = new ReactiveCollection<SkillCountEntity>();
            WarStatus         = new ReactivePropertySlim<WarEvents>(WarEvents.Invalid);
            AverageFps        = new ReactivePropertySlim<double>(0d);
            Pow               = new ReactivePropertySlim<int>(0);
            PowDebuffs        = new ReactivePropertySlim<string>(string.Empty);
        }

        public static async Task<SkillCountUseCase> CreateAsync()
        {
            var ret = new SkillCountUseCase();

            await ret.CreateAsync(DbFilePath);

            return ret;
        }

        private async Task CreateAsync(string dbFilePath)
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

            _skillCountRepository = await SkillCountRepository.CreateAsync(dbFilePath);
            SkillCountHitory.AddRangeOnScheduler(_skillCountRepository.GetSkillCounts());
        }

        public void StartSkillCounter()
        {

        }

        public void StopSkillCounter()
        {

        }

        public void ResetSkillCount()
        {
            foreach (var d in CurrentSkillCount.Value.Details)
            {
                d.Reset();
            }

            UpdateSkillText();
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

        private void _skillUseService_WarFinished(object sender, Map e)
        {
            WarStatus.Value = WarEvents.WarFinished;
            MapName.Value   = e.IsEmpty() ? string.Empty : e.Name;

            SaveSkillCount();
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
            bool requireUpdate = false;
            if (skills != null)
            {
                foreach (var s in skills.Where(x => !x.IsEmpty()))
                {
                    if (!CurrentSkillCount.Value.Details.Any(x => x.SkillName == s.Name))
                    {
                        var detail = new SkillCountDetailEntity()
                        {
                            SkillName      = s.Name,
                            SkillShortName = s.ShortName
                        };
                        CurrentSkillCount.Value.Details.Add(detail);
                        requireUpdate = true;
                    }
                }
            }
            else
            {
                CurrentSkillCount.Value = new SkillCountEntity();
                requireUpdate = true;
            }

            if (requireUpdate)
            {
                UpdateSkillText();
            }
        }

        private void _skillUseService_SkillCountIncremented(object sender, Skill skill)
        {
            var skillCount = CurrentSkillCount.Value.Details.FirstOrDefault(x => x.SkillName == skill.Name);
            if (skillCount != null)
            {
                skillCount.Increment();

                UpdateSkillText();
            }
        }

        private void UpdateSkillText()
        {
            var text = string.Join(
                Environment.NewLine,
                CurrentSkillCount.Value.Details.Select(x => x.SkillShortName + "：" + x.Count));

            using (var sw = new StreamWriter("skillcount.txt", false, Encoding.UTF8))
            {
                sw.WriteLine(text);
                sw.Flush();
            }
        }

        private void SaveSkillCount()
        {
            if (!CurrentSkillCount.Value.Details.Any())
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
