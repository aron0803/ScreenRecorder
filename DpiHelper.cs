using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace ScreenRecorder
{
    /// <summary>
    /// Helper class for DPI-aware coordinate conversion
    /// </summary>
    public static class DpiHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromPoint(Point pt, uint dwFlags);

        [DllImport("shcore.dll")]
        private static extern int GetDpiForMonitor(IntPtr hmonitor, DpiType dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        private const uint MONITOR_DEFAULTTONEAREST = 2;

        public enum DpiType
        {
            Effective = 0,
            Angular = 1,
            Raw = 2,
        }

        /// <summary>
        /// Get DPI scaling factor for a specific point on screen
        /// </summary>
        public static double GetDpiScaleForPoint(Point point)
        {
            try
            {
                IntPtr monitor = MonitorFromPoint(point, MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    GetDpiForMonitor(monitor, DpiType.Effective, out uint dpiX, out uint dpiY);
                    return dpiX / 96.0; // 96 DPI is 100% scaling
                }
            }
            catch
            {
                // Fall back to 1.0 if API not available
            }
            return 1.0;
        }

        /// <summary>
        /// Get DPI scaling factor for a specific rectangle
        /// Uses the center point of the rectangle
        /// </summary>
        public static double GetDpiScaleForRectangle(Rectangle rect)
        {
            Point center = new Point(
                rect.Left + rect.Width / 2,
                rect.Top + rect.Height / 2
            );
            return GetDpiScaleForPoint(center);
        }

        /// <summary>
        /// Convert logical coordinates to physical (screen) coordinates
        /// </summary>
        public static Rectangle LogicalToPhysical(Rectangle logicalRect)
        {
            double scale = GetDpiScaleForRectangle(logicalRect);
            if (Math.Abs(scale - 1.0) < 0.001) // No scaling needed
                return logicalRect;

            return new Rectangle(
                (int)Math.Round(logicalRect.X * scale),
                (int)Math.Round(logicalRect.Y * scale),
                (int)Math.Round(logicalRect.Width * scale),
                (int)Math.Round(logicalRect.Height * scale)
            );
        }

        /// <summary>
        /// Convert physical (screen) coordinates to logical coordinates
        /// </summary>
        public static Rectangle PhysicalToLogical(Rectangle physicalRect)
        {
            double scale = GetDpiScaleForRectangle(physicalRect);
            if (Math.Abs(scale - 1.0) < 0.001) // No scaling needed
                return physicalRect;

            return new Rectangle(
                (int)Math.Round(physicalRect.X / scale),
                (int)Math.Round(physicalRect.Y / scale),
                (int)Math.Round(physicalRect.Width / scale),
                (int)Math.Round(physicalRect.Height / scale)
            );
        }

        /// <summary>
        /// Check if the system is using DPI scaling
        /// </summary>
        public static bool IsScaled()
        {
            double scale = GetDpiScaleForPoint(new Point(0, 0));
            return Math.Abs(scale - 1.0) > 0.001;
        }

        /// <summary>
        /// Get current DPI scale percentage (e.g., 125, 150)
        /// </summary>
        public static int GetScalePercentage()
        {
            double scale = GetDpiScaleForPoint(new Point(0, 0));
            return (int)Math.Round(scale * 100);
        }
    }
}
