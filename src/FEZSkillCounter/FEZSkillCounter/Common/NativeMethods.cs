using System.Runtime.InteropServices;

namespace FEZSkillCounter.Common
{
    internal static class NativeMethods
    {
        [DllImport("Kernel32.dll")]
        public static extern bool AttachConsole(int processId);
    }
}
