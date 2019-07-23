using SkillUseCounter.Entity;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FEZSkillCounter.Entity
{
    public class SkillCount : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _shorName = string.Empty;
        public string ShortName
        {
            get { return _shorName; }
            set { SetProperty(ref _shorName, value); }
        }

        private int _count = 0;
        public int Count
        {
            get { return _count; }
            set { SetProperty(ref _count, value); }
        }

        public SkillCount(Skill skill)
        {
            Name      = skill.Name;
            ShortName = skill.ShortName;
            Count     = 0;
        }

        public void Increment()
        {
            Count++;
        }

        public void Reset()
        {
            Count = 0;
        }

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return true;
        }
    }
}
