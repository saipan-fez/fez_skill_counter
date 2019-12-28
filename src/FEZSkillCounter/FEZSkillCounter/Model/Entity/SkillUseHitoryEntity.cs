using FEZSkillCounter.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace FEZSkillCounter.Model.Entity
{
    public class SkillUseEntity : BindableBase
    {
        [Key]
        public int SkillUseId { get { return _skillUseId; } set { SetProperty(ref _skillUseId, value); } }
        private int _skillUseId;

        public DateTime UseDate { get { return _useDate; } set { SetProperty(ref _useDate, value); } }
        private DateTime _useDate = DateTime.MinValue;

        public string SkillName { get { return _skillName; } set { SetProperty(ref _skillName, value); } }
        private string _skillName = "Unknown";

        public string SkillShortName { get { return _skillShortName; } set { SetProperty(ref _skillShortName, value); } }
        private string _skillShortName = "Unknown";

        public SkillCountEntity Parent { get { return _parent; } set { SetProperty(ref _parent, value); } }
        private SkillCountEntity _parent = null;
    }
}
