using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ScreenRecorder
{
    public class KeyOverlayForm : Form
    {
        private Label lblKeys;
        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private System.Windows.Forms.Timer clearTimer;

        public KeyOverlayForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.StartPosition = FormStartPosition.Manual;
            this.Size = new Size(800, 100); // Initial size, will adjust
            
            // Enable double buffering to reduce flicker
            this.DoubleBuffered = true;

            lblKeys = new Label();
            lblKeys.AutoSize = true;
            lblKeys.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblKeys.ForeColor = Color.White;
            lblKeys.BackColor = Color.FromArgb(180, 0, 0, 0); // Semi-transparent black background for text
            lblKeys.Padding = new Padding(10);
            lblKeys.Location = new Point(10, 10);
            lblKeys.Text = "";
            lblKeys.Visible = false;
            this.Controls.Add(lblKeys);

            clearTimer = new System.Windows.Forms.Timer();
            clearTimer.Interval = 2000; // Clear after 2 seconds of inactivity if needed, though logic says "until release"
            clearTimer.Tick += ClearTimer_Tick;
        }

        // Prevent form from stealing focus
        protected override bool ShowWithoutActivation => true;

        private void ClearTimer_Tick(object sender, EventArgs e)
        {
            // Optional: clear if stuck, but main logic is driven by key up
        }

        public void SetPosition(Rectangle region)
        {
            // Position at bottom-left of the region
            // Ensure we don't go off screen if possible, but strictly following "bottom-left of region"
            
            // If region is empty (fullscreen), use primary screen
            if (region.IsEmpty)
            {
                region = Screen.PrimaryScreen.Bounds;
            }

            // Calculate position: Bottom-Left
            // We give it some padding from the edge
            int x = region.Left + 20;
            int y = region.Bottom - 100; // Approximate height of our form/label

            this.Location = new Point(x, y);
        }

        public void OnKeyDown(Keys key)
        {
            if (pressedKeys.Contains(key)) return;
            
            pressedKeys.Add(key);
            UpdateDisplay();
        }

        public void OnKeyUp(Keys key)
        {
            if (pressedKeys.Contains(key))
            {
                pressedKeys.Remove(key);
                // If all keys released, clear display after a short delay or immediately?
                // User said: "until release". 
                // "多個按鍵就依按下去的顯示" -> Show as long as pressed.
                
                if (pressedKeys.Count == 0)
                {
                    // All keys released, clear text
                    lblKeys.Text = "";
                    lblKeys.Visible = false;
                }
                else
                {
                    UpdateDisplay();
                }
            }
        }

        // Clear all pressed keys (useful when system dialogs like Ctrl+Alt+Del steal focus)
        public void ClearAllKeys()
        {
            if (pressedKeys.Count > 0)
            {
                pressedKeys.Clear();
                lblKeys.Text = "";
                lblKeys.Visible = false;
            }
        }

        private void UpdateDisplay()
        {
            if (pressedKeys.Count == 0) return;

            // Get formatted names and remove duplicates (e.g. LShift + RShift -> Shift)
            var keyNames = pressedKeys.Select(k => FormatKey(k)).Distinct().ToList();

            // Define sort order for modifiers
            var sortOrder = new List<string> { "Ctrl", "Shift", "Alt", "Win" };

            // Sort: Modifiers first (in specific order), then others
            keyNames.Sort((a, b) => {
                int indexA = sortOrder.IndexOf(a);
                int indexB = sortOrder.IndexOf(b);

                if (indexA != -1 && indexB != -1) return indexA.CompareTo(indexB);
                if (indexA != -1) return -1;
                if (indexB != -1) return 1;
                
                return string.Compare(a, b, StringComparison.Ordinal);
            });

            string text = string.Join(" + ", keyNames);
            
            lblKeys.Text = text;
            lblKeys.Visible = true;
            this.Width = Math.Max(800, lblKeys.Width + 20);
        }

        private string FormatKey(Keys key)
        {
            // Handle modifiers
            if (key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.Control || key == Keys.ControlKey) return "Ctrl";
            if (key == Keys.LShiftKey || key == Keys.RShiftKey || key == Keys.Shift || key == Keys.ShiftKey) return "Shift";
            if (key == Keys.LMenu || key == Keys.RMenu || key == Keys.Alt || key == Keys.Menu) return "Alt";
            if (key == Keys.LWin || key == Keys.RWin) return "Win";

            // Handle numbers
            if (key >= Keys.D0 && key <= Keys.D9) return ((int)key - (int)Keys.D0).ToString();
            if (key >= Keys.NumPad0 && key <= Keys.NumPad9) return ((int)key - (int)Keys.NumPad0).ToString();

            // Handle function keys
            if (key >= Keys.F1 && key <= Keys.F12) return key.ToString();

            // Handle special keys
            switch (key)
            {
                case Keys.Back: return "Back";
                case Keys.Return: return "Enter";
                case Keys.Capital: return "Caps";
                case Keys.Space: return "Space";
                case Keys.Prior: return "PgUp";
                case Keys.Next: return "PgDn";
                case Keys.End: return "End";
                case Keys.Home: return "Home";
                case Keys.Insert: return "Ins";
                case Keys.Delete: return "Del";
                case Keys.Escape: return "Esc";
                case Keys.Tab: return "Tab";
                case Keys.Multiply: return "*";
                case Keys.Add: return "+";
                case Keys.Subtract: return "-";
                case Keys.Divide: return "/";
                case Keys.Decimal: return ".";
                
                case Keys.Oemcomma: return ",";
                case Keys.OemPeriod: return ".";
                case Keys.OemQuestion: return "/";
                case Keys.Oemplus: return "=";
                case Keys.OemMinus: return "-";
                case Keys.Oem1: return ";";
                case Keys.Oem7: return "'";
                case Keys.OemOpenBrackets: return "[";
                case Keys.OemCloseBrackets: return "]";
                case Keys.Oem5: return "\\";
                case Keys.Oemtilde: return "`";

                default: return key.ToString();
            }
        }
    }
}
