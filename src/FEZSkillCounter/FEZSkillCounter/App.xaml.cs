using Microsoft.Extensions.Logging;
using SkillUseCounter;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace FEZSkillUseCounter
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
#if DEBUG
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        static App()
        {
            AttachConsole(-1);
        }
#endif

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.FirstChanceException += (s, args) =>
            {
                Logger.WriteException(args.Exception);
            };
        }
    }
}
