using System;
using System.Windows;
using System.Windows.Interactivity;

namespace FEZSkillCounter.View.Behavior
{
    public class ViewModelDisposeBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closed += this.WindowClosed;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            (AssociatedObject.DataContext as IDisposable)?.Dispose();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closed -= WindowClosed;
        }
    }
}
