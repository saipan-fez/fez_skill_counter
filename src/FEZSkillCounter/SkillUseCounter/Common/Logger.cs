using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using System.Text;

namespace SkillUseCounter
{
    internal static class Logger
    {
        private static StreamWriter _logWriter = null;

        public static void WriteLine(string msg, [CallerMemberName] string memberName = "")
        {
            if (_logWriter == null)
            {
                _logWriter = CreateLogWriter();
            }

            var d = DateTime.Now;
            msg = $"[{d.Hour.ToString("D2")}:{d.Minute.ToString("D2")}:{d.Second.ToString("D2")}.{d.Millisecond.ToString("D3")}]{msg} ({memberName})";

            _logWriter.WriteLine(msg);
            _logWriter.Flush();
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }

        private static StreamWriter CreateLogWriter()
        {
            var assembly     = Assembly.GetEntryAssembly();
            var exeDirectory = Path.GetDirectoryName(assembly.Location);
            var logDirectory = Path.Combine(exeDirectory, "log");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            var logPath = Path.Combine(logDirectory, $"{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.log");

            return new StreamWriter(logPath, false, Encoding.UTF8);
        }
    }
}
