using FEZSkillCounter.Common;
using FEZSkillCounter.Model.Repository;
using Microsoft.EntityFrameworkCore;
using SkillUseCounter;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

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
        static App()
        {
            NativeMethods.AttachConsole(-1);
        }
#endif

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.FirstChanceException += (s, args) =>
            {
                Logger.WriteException(args.Exception);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                DeleteMahAppSettingFileIfError(args);
            };

            AppDb = new AppDbContext(DbFilePath);
            await AppDb.Database.MigrateAsync();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppDb.Dispose();
            AppDb = null;
        }

        private void DeleteMahAppSettingFileIfError(DispatcherUnhandledExceptionEventArgs args)
        {
            // MahAppのウィンドウサイズ・位置を保持している user.config が、
            // 時折破損することがある。
            // そのため、破損(ConfigurationErrorsException)を検知した場合はuser.configを削除する。
            // なお、例外をキャッチ後はアプリを再起動しないと上手く起動しないため、
            // メッセージ表示後にアプリを終了する。
            var ex = args.Exception?.InnerException as ConfigurationErrorsException;
            if (ex != null && Path.GetFileName(ex.Filename) == "user.config")
            {
                try
                {
                    File.Delete(ex.Filename);

                    MessageBox.Show(
                        "設定ファイルが破損していました。" + Environment.NewLine +
                        "アプリを再起動してください。",
                        "Error");
                }
                catch
                {
                    MessageBox.Show(
                        "設定ファイルが破損していました。" + Environment.NewLine +
                        "手動で下記のファイルを削除してください。" + Environment.NewLine +
                        ex.Filename,
                        "Error");
                }
                finally
                {
                    Shutdown();
                }
            }
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
