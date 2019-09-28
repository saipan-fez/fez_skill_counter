using FEZSkillCounter.Common;
using FEZSkillCounter.Model.Repository;
using Microsoft.EntityFrameworkCore;
using SkillUseCounter;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        [STAThread]
        public static void Main()
        {
            using (var semaphore = new Semaphore(1, 1, "FEZSkillCounter", out bool createdNew))
            {
                if (!createdNew)
                {
                    return;
                }

                var isValid = FEZCommonLibrary.FEZSettingValidator.ValidateGlobalIniSetting();
                if (isValid)
                {
                    var app = new App();
                    app.InitializeComponent();
                    app.Run();
                }
                else
                {
                    MessageBox.Show(
                        "GLOBAL.iniの設定内容がツールに適していません。" + Environment.NewLine +
                        "下記の設定を見直してください。" + Environment.NewLine +
                        "" + Environment.NewLine +
                        "・フルスクリーン：OFF" + Environment.NewLine +
                        "・ウィンドウカラー：通常",
                        "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException               += Application_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException      += TaskScheduler_UnobservedTaskException;

            AppDb = new AppDbContext(DbFilePath);
            await AppDb.Database.MigrateAsync();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is TaskCanceledException) return;

            ShutdownIfMahAppSettingFileError(e);

            ApplicationError.HandleUnexpectedError(e.Exception);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ApplicationError.HandleUnexpectedError(e.Exception);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var showMessageBox = !e.IsTerminating;

            ApplicationError.HandleUnexpectedError(e.ExceptionObject as Exception, showMessageBox);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppDb.Dispose();
            AppDb = null;
        }

        private void ShutdownIfMahAppSettingFileError(DispatcherUnhandledExceptionEventArgs args)
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
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch
                {
                    MessageBox.Show(
                        "設定ファイルが破損していました。" + Environment.NewLine +
                        "手動で下記のファイルを削除してください。" + Environment.NewLine +
                        ex.Filename,
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    Shutdown(-1);
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

    public class ApplicationError
    {
        private const string ErrorLogFileName = "error_{0}.log";

        private static readonly DirectoryInfo _directory = new DirectoryInfo(
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "error"));

        private static bool _isFirstError = true;

        public static void HandleUnexpectedError(Exception ex, bool showMessageBox = true)
        {
            if (!_isFirstError)
            {
                return;
            }

            if (ex is FileNotFoundException && ex.Message.IndexOf("ControlzEx.XmlSerializers") != -1)
            {
                // 起動時に毎回スローされるが、特に害はない例外のため記録しない
                return;
            }

#if DEBUG
            Debugger.Break();
#endif

            _isFirstError = true;

            try
            {
                OutputAsLogFile(ex);
            }
            catch { }

            if (showMessageBox)
            {
                string errorMsg =
                    "予期せぬエラーが発生しました。" + Environment.NewLine +
                    "アプリケーションを終了します。" + Environment.NewLine +
                    "(errorlogフォルダにエラー内容が保存されます）";

                MessageBox.Show(errorMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Application.Current.Shutdown(-1);
        }

        private static void OutputAsLogFile(Exception ex)
        {
            if (ex == null)
            {
                return;
            }

            if (!_directory.Exists)
            {
                _directory.Create();
            }

            var logfileName = string.Format(ErrorLogFileName, DateTime.Now.ToString("yyyyMMddHHmmss"));
            var fullPath = Path.Combine(_directory.FullName, logfileName);

            using (var sw = new StreamWriter(fullPath, false, Encoding.UTF8))
            {
                sw.WriteLine("---Exception------------------------");
                sw.WriteLine("[Message]");
                sw.WriteLine(ex.Message);
                sw.WriteLine("[Source]");
                sw.WriteLine(ex.Source);
                sw.WriteLine("[StackTrace]");
                sw.WriteLine(ex.StackTrace);
                sw.WriteLine(string.Empty);

                OutputInnnerException(ex, sw);
            }
        }

        private static void OutputInnnerException(Exception ex, StreamWriter sw)
        {
            if (ex.InnerException == null)
            {
                return;
            }

            sw.WriteLine("---InnerException-------------------");
            sw.WriteLine("[Message]");
            sw.WriteLine(ex.InnerException.Message);
            sw.WriteLine("[Source]");
            sw.WriteLine(ex.InnerException.Source);
            sw.WriteLine("[StackTrace]");
            sw.WriteLine(ex.InnerException.StackTrace);
            sw.WriteLine(string.Empty);

            OutputInnnerException(ex.InnerException, sw);
        }
    }
}
