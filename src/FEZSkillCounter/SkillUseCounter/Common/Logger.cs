using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEZSkillUseCounter
{
    internal static class Logger
    {
        public static void WriteLine(string msg)
        {
            var d = DateTime.Now;
            msg = $"[{d.Hour.ToString("D2")}:{d.Minute.ToString("D2")}:{d.Second.ToString("D2")}.{d.Millisecond.ToString("D3")}]{msg}";

            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }
    }
}
