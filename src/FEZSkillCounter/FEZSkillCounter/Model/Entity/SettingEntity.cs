using FEZSkillCounter.Model.Common;
using System;
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

        public bool IsNotifyEnchantUses { get { return _isNotifyEnchantUses; } set { SetProperty(ref _isNotifyEnchantUses, value); } }
        private bool _isNotifyEnchantUses = true;

        public bool IsNotifySpellUses { get { return _isNotifySpellUses; } set { SetProperty(ref _isNotifySpellUses, value); } }
        private bool _isNotifySpellUses = true;

        public TimeSpan? EnchantSpellNotifyTimeSpan { get { return _enchantSpellNotifyTimeSpan; } set { SetProperty(ref _enchantSpellNotifyTimeSpan, value); } }
        private TimeSpan? _enchantSpellNotifyTimeSpan = TimeSpan.FromSeconds(10);

        public bool IsAllwaysOnTop { get { return _isAllwaysOnTop; } set { SetProperty(ref _isAllwaysOnTop, value); } }
        private bool _isAllwaysOnTop = false;

        public bool IsDebugModeEnabled { get { return _isDebugModeEnabled; } set { SetProperty(ref _isDebugModeEnabled, value); } }
        private bool _isDebugModeEnabled = false;
    }
}
