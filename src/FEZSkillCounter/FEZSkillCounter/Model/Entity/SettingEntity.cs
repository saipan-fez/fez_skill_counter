using FEZSkillCounter.Model.Common;
using System.ComponentModel.DataAnnotations;

namespace FEZSkillCounter.Model.Entity
{
    public class SettingEntity : BindableBase
    {
        [Key]
        public int Id { get; set; }

        public bool IsSkillCountFileSave { get { return _isSkillCountFileSave; } set { SetProperty(ref _isSkillCountFileSave, value); } }
        private bool _isSkillCountFileSave = true;

        public bool IsNotifyBookUses { get { return _isNotifyBookUses; } set { SetProperty(ref _isNotifyBookUses, value); } }
        private bool _isNotifyBookUses = true;

        public bool IsDebugModeEnabled { get { return _isDebugModeEnabled; } set { SetProperty(ref _isDebugModeEnabled, value); } }
        private bool _isDebugModeEnabled = false;
    }
}
