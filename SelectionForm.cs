using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenRecorder
{
    public class SelectionForm : Form
    {
        public Rectangle SelectedRegion { get; private set; }
        private Point startPoint;
        private bool isSelecting;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point p);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public SelectionForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            // Support multiple monitors with different resolutions
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = SystemInformation.VirtualScreen;
            
            this.DoubleBuffered = true;
            this.Cursor = Cursors.Cross;
            this.BackColor = Color.Black;
            this.Opacity = 0.3;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            
            // Add instruction label
            Label lblInstruction = new Label();
            lblInstruction.Text = "拖曳選取範圍 或 點擊視窗自動選取\nESC 取消";
            lblInstruction.AutoSize = true;
            lblInstruction.ForeColor = Color.White;
            lblInstruction.BackColor = Color.FromArgb(180, 0, 0, 0);
            lblInstruction.Padding = new Padding(10);
            lblInstruction.Font = new Font("Microsoft JhengHei", 12, FontStyle.Bold);
            lblInstruction.Location = new Point(20, 20);
            this.Controls.Add(lblInstruction);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isSelecting = true;
                startPoint = e.Location;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isSelecting)
            {
                this.Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isSelecting)
            {
                isSelecting = false;
                
                // Check if it was a click (small movement)
                if (Math.Abs(startPoint.X - e.Location.X) < 5 && Math.Abs(startPoint.Y - e.Location.Y) < 5)
                {
                    // Single click - select window under cursor
                    // Temporarily hide to get window underneath
                    this.Visible = false;
                    System.Threading.Thread.Sleep(10);
                    
                    IntPtr hWnd = WindowFromPoint(Cursor.Position);
                    
                    this.Visible = true;
                    
                    if (hWnd != IntPtr.Zero)
                    {
                        RECT rect;
                        if (GetWindowRect(hWnd, out rect))
                        {
                            // GetWindowRect returns physical coordinates
                            // Store them directly as they are already in screen coordinates
                            SelectedRegion = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                        }
                        else
                        {
                            SelectedRegion = Screen.PrimaryScreen.Bounds;
                        }
                    }
                    else
                    {
                         SelectedRegion = Screen.PrimaryScreen.Bounds;
                    }
                }
                else
                {
                    // Drag selection - these are already in screen coordinates
                    SelectedRegion = GetRectangle(startPoint, e.Location);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            
            if (isSelecting)
            {
                Rectangle rect = GetRectangle(startPoint, this.PointToClient(Cursor.Position));
                using (Pen pen = new Pen(Color.Red, 3))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        private Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.Y - p2.Y));
        }
        
        // Allow exiting with Esc
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
