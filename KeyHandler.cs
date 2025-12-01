using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ScreenRecorder
{
    public static class KeyHandler
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const int WM_HOTKEY = 0x0312;

        public static void Register(IntPtr hWnd, int id, Keys key)
        {
            // fsModifiers: 0 = None, 1 = Alt, 2 = Control, 4 = Shift, 8 = Windows
            RegisterHotKey(hWnd, id, 0, (int)key);
        }

        public static void Unregister(IntPtr hWnd, int id)
        {
            UnregisterHotKey(hWnd, id);
        }
    }
}
