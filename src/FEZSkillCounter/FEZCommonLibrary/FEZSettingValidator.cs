using System;
using System.IO;
using System.Text;

namespace FEZCommonLibrary
{
    /// <summary>
    /// FEZの設定確認クラス
    /// </summary>
    public class FEZSettingValidator
    {
        /// <summary>
        /// GLOBAL.INIの設定がツールに適しているか検証する
        /// </summary>
        /// <returns></returns>
        public static bool ValidateGlobalIniSetting()
        {
            var globalIniPath = Path.Combine(
                Environment.Is64BitOperatingSystem ?
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) :
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "SquareEnix\\FantasyEarthZero\\Settings\\GLOBAL.INI");

            // TODO: 確認するフォルダをインストール先に対応させる(現状デフォルトのみ対応)
            // インストール先にGLOBAL.iniが存在しない場合はチェックをスルー
            if (!File.Exists(globalIniPath))
            {
                return true;
            }

            var fullscreen  = GetIniValue(globalIniPath, "GLOBAL", "FULLSCREEN");
            var windowColor = GetIniValue(globalIniPath, "GLOBAL", "WINDOW_COLOR");

            return
                fullscreen  == "0" &&   // ウィンドウモード
                windowColor == "1";     // カラー1
        }

        private static string GetIniValue(string path, string section, string key)
        {
            var sb = new StringBuilder(256);
            NativeMethods.GetPrivateProfileString(section, key, string.Empty, sb, sb.Capacity, path);
            return sb.ToString();
        }
    }
}
