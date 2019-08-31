using FEZSkillCounter.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace FEZSkillCounter.Model.Entity
{
    public class SkillCountDetailEntity : BindableBase
    {
        [Key]
        public int SkillCountDetailId { get { return _skillCountDetailId; } set { SetProperty(ref _skillCountDetailId, value); } }
        private int _skillCountDetailId;

        public string SkillName { get { return _skillName; } set { SetProperty(ref _skillName, value); } }
        private string _skillName = string.Empty;

        public string SkillShortName { get { return _skillShortName; } set { SetProperty(ref _skillShortName, value); } }
        private string _skillShortName = string.Empty;

        public string WorkName { get { return _workName; } set { SetProperty(ref _workName, value); } }
        private string _workName = string.Empty;

        public int Count { get { return _count; } set { SetProperty(ref _count, value); } }
        private int _count = 0;

        public SkillCountEntity Parent { get { return _parent; } set { SetProperty(ref _parent, value); } }
        private SkillCountEntity _parent = null;
    }
}
