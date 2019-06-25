using System.Runtime.InteropServices;
using System.Windows;

namespace FEZSkillCounter
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        static App()
        {
            AttachConsole(-1);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            SkillStorage.Create();
            PowStorage.Create();
        }
    }
}
