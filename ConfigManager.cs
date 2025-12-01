using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ScreenRecorder
{
    public static class ConfigManager
    {
        private static string ConfigPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public static void SaveSettings(int fps, int bitrateIndex, bool isRegion, Rectangle region)
        {
            WritePrivateProfileString("Settings", "FPS", fps.ToString(), ConfigPath);
            WritePrivateProfileString("Settings", "BitrateIndex", bitrateIndex.ToString(), ConfigPath);
            WritePrivateProfileString("Settings", "IsRegion", isRegion.ToString(), ConfigPath);
            WritePrivateProfileString("Settings", "RegionX", region.X.ToString(), ConfigPath);
            WritePrivateProfileString("Settings", "RegionY", region.Y.ToString(), ConfigPath);
            WritePrivateProfileString("Settings", "RegionW", region.Width.ToString(), ConfigPath);
            WritePrivateProfileString("Settings", "RegionH", region.Height.ToString(), ConfigPath);
        }

        public static void LoadSettings(out int fps, out int bitrateIndex, out bool isRegion, out Rectangle region)
        {
            fps = ParseInt("FPS", 30);
            bitrateIndex = ParseInt("BitrateIndex", 0); // 0: Low, 1: Medium, 2: High
            isRegion = ParseBool("IsRegion", false);
            
            int x = ParseInt("RegionX", 0);
            int y = ParseInt("RegionY", 0);
            int w = ParseInt("RegionW", 0);
            int h = ParseInt("RegionH", 0);
            region = new Rectangle(x, y, w, h);
        }

        private static int ParseInt(string key, int defaultValue)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString("Settings", key, defaultValue.ToString(), sb, 255, ConfigPath);
            if (int.TryParse(sb.ToString(), out int result))
                return result;
            return defaultValue;
        }

        private static bool ParseBool(string key, bool defaultValue)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString("Settings", key, defaultValue.ToString(), sb, 255, ConfigPath);
            if (bool.TryParse(sb.ToString(), out bool result))
                return result;
            return defaultValue;
        }
    }
}
