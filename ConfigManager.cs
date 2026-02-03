using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ScreenRecorder
{
    public static class ConfigManager
    {
        private const string RegKeyPath = @"Software\ScreenRecorder";

        public static void SaveSettings(int fps, int bitrateIndex, bool isRegion, Rectangle region)
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegKeyPath))
                {
                    if (key != null)
                    {
                        key.SetValue("FPS", fps);
                        key.SetValue("BitrateIndex", bitrateIndex);
                        key.SetValue("IsRegion", isRegion ? 1 : 0);
                        key.SetValue("RegionX", region.X);
                        key.SetValue("RegionY", region.Y);
                        key.SetValue("RegionW", region.Width);
                        key.SetValue("RegionH", region.Height);
                    }
                }
            }
            catch { /* Ignore registry errors */ }
        }

        public static void LoadSettings(out int fps, out int bitrateIndex, out bool isRegion, out Rectangle region)
        {
            fps = 30;
            bitrateIndex = 0;
            isRegion = false;
            int x = 0, y = 0, w = 0, h = 0;

            try
            {
                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegKeyPath))
                {
                    if (key != null)
                    {
                        fps = (int)key.GetValue("FPS", 30);
                        bitrateIndex = (int)key.GetValue("BitrateIndex", 0);
                        isRegion = (int)key.GetValue("IsRegion", 0) == 1;
                        x = (int)key.GetValue("RegionX", 0);
                        y = (int)key.GetValue("RegionY", 0);
                        w = (int)key.GetValue("RegionW", 0);
                        h = (int)key.GetValue("RegionH", 0);
                    }
                }
            }
            catch { /* Use defaults on error */ }

            region = new Rectangle(x, y, w, h);
        }
    }
}
