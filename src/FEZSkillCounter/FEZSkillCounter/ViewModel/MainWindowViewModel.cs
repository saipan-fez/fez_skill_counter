using Reactive.Bindings;

namespace FEZSkillCounter.ViewModel
{
    public class MainWindowViewModel
    {
        public ReactiveProperty<string> MapName { get; } = new ReactiveProperty<string>();

        public MainWindowViewModel()
        {

        }
    }
}
