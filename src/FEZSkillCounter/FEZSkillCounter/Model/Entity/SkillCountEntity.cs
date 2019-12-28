using FEZSkillCounter.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FEZSkillCounter.Model.Entity
{
    public class SkillCountEntity : BindableBase
    {
        [Key]
        public int SkillCountId { get { return _skillCountId; } set { SetProperty(ref _skillCountId, value); } }
        private int _skillCountId = 0;

        public DateTime RecordDate { get { return _recordDate; } set { SetProperty(ref _recordDate, value); } }
        private DateTime _recordDate = DateTime.MinValue;

        public string MapName { get { return _mapName; } set { SetProperty(ref _mapName, value); } }
        private string _mapName = "Unknown";

        public string WorkName { get { return _workName; } set { SetProperty(ref _workName, value); } }
        private string _workName = "Unknown";

        public List<SkillCountDetailEntity> Details { get; set; } = new List<SkillCountDetailEntity>();
        public List<SkillUseEntity> SkillUseHitories { get; set; } = new List<SkillUseEntity>();
    }
}
