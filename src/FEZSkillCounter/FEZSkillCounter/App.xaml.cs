using FEZSkillCounter.Model.Repository;
using Microsoft.EntityFrameworkCore;
using SkillUseCounter;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

namespace FEZSkillCounter
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private static readonly string DbFilePath  = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "skillcount.db");

        public AppDbContext AppDb { get; private set; }

#if DEBUG
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);

        static App()
        {
            AttachConsole(-1);
        }
#endif

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.FirstChanceException += (s, args) =>
            {
                Logger.WriteException(args.Exception);
            };

            AppDb = new AppDbContext(DbFilePath);
            await AppDb.Database.MigrateAsync();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppDb.Dispose();
            AppDb = null;
        }
    }

    public static class ApplicationExtension
    {
        public static AppDbContext GetAppDb(this Application application)
        {
            var app = application as App;
            return app?.AppDb;
        }
    }
}
