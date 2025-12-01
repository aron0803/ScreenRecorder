using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ScreenRecorder
{
    // Helper class to display filename but store full path
    public class FileItem
    {
        public string FullPath { get; set; }
        public override string ToString() => System.IO.Path.GetFileName(FullPath);
    }

    public partial class MainForm : Form
    {
        private Recorder recorder;
        private const int HOTKEY_ID_START = 1;
        private const int HOTKEY_ID_SCREENSHOT = 2;
        private const int HOTKEY_ID_AUDIO = 3;
        private System.Drawing.Rectangle currentRegion;
        private KeyOverlayForm keyOverlay;

        private string RecordFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "record");
        private string VideoFolder => Path.Combine(RecordFolder, "Videos");
        private string ScreenshotFolder => Path.Combine(RecordFolder, "Screenshots");
        private string AudioFolder => Path.Combine(RecordFolder, "Audio");

        public MainForm()
        {
            InitializeComponent();
            recorder = new Recorder();
            this.Load += MainForm_Load;
            this.FormClosing += MainForm_FormClosing;
            this.Activated += MainForm_Activated;
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            // Clear stuck keys when window is activated (e.g., after Ctrl+Alt+Del)
            keyOverlay?.ClearAllKeys();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            KeyHandler.Register(this.Handle, HOTKEY_ID_START, Keys.F9);
            KeyHandler.Register(this.Handle, HOTKEY_ID_SCREENSHOT, Keys.F10);
            KeyHandler.Register(this.Handle, HOTKEY_ID_AUDIO, Keys.F8);
            
            // Create record folders if they don't exist
            if (!Directory.Exists(RecordFolder))
                Directory.CreateDirectory(RecordFolder);
            if (!Directory.Exists(VideoFolder))
                Directory.CreateDirectory(VideoFolder);
            if (!Directory.Exists(ScreenshotFolder))
                Directory.CreateDirectory(ScreenshotFolder);
            if (!Directory.Exists(AudioFolder))
                Directory.CreateDirectory(AudioFolder);
            
            LoadSettings();
            LoadExistingFiles();
            InitializeContextMenu();
            try
            {
                InitializeKeyOverlay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"按鍵顯示功能初始化失敗: {ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            UpdateStatus("就緒 - F8:錄音 F9:錄影 F10:截圖");
        }

        private void InitializeKeyOverlay()
        {
            keyOverlay = new KeyOverlayForm();
            // Show it so it can receive updates (it manages its own visibility of text)
            keyOverlay.Show(); 
            
            GlobalKeyboardHook.KeyDown += (s, k) => {
                if (InvokeRequired) Invoke(new Action(() => keyOverlay.OnKeyDown(k)));
                else keyOverlay.OnKeyDown(k);
            };
            
            GlobalKeyboardHook.KeyUp += (s, k) => {
                if (InvokeRequired) Invoke(new Action(() => keyOverlay.OnKeyUp(k)));
                else keyOverlay.OnKeyUp(k);
            };
            
            GlobalKeyboardHook.Start();
            UpdateOverlayPosition();
            UpdateKeyOverlayVisibility();
        }

        private void UpdateKeyOverlayVisibility()
        {
            if (keyOverlay == null || keyOverlay.IsDisposed) return;
            keyOverlay.Visible = chkShowKeys.Checked;
        }

        private void UpdateOverlayPosition()
        {
            if (keyOverlay == null || keyOverlay.IsDisposed) return;

            Rectangle targetRegion;
            if (rbRegion.Checked && currentRegion.Width > 0 && currentRegion.Height > 0)
            {
                targetRegion = currentRegion;
            }
            else
            {
                targetRegion = Screen.PrimaryScreen.Bounds;
            }
            
            keyOverlay.SetPosition(targetRegion);
        }

        private void InitializeContextMenu()
        {
            var contextMenu = new ContextMenuStrip();
            var deleteItem = new ToolStripMenuItem("刪除");
            deleteItem.Click += DeleteFile_Click;
            contextMenu.Items.Add(deleteItem);
            lstFiles.ContextMenuStrip = contextMenu;
        }

        private void DeleteFile_Click(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItem is FileItem fileItem)
            {
                var result = MessageBox.Show($"確定要刪除檔案 '{fileItem}' 嗎？", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        if (File.Exists(fileItem.FullPath))
                        {
                            File.Delete(fileItem.FullPath);
                        }
                        lstFiles.Items.Remove(fileItem);
                        UpdateStatus($"已刪除檔案: {fileItem}");
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"刪除失敗: {ex.Message}");
                    }
                }
            }
        }

        private void LoadSettings()
        {
            ConfigManager.LoadSettings(out int fps, out int bitrateIndex, out bool isRegion, out System.Drawing.Rectangle region);
            
            numFps.Value = Math.Max(numFps.Minimum, Math.Min(numFps.Maximum, fps));
            if (bitrateIndex >= 0 && bitrateIndex < cmbBitrate.Items.Count)
                cmbBitrate.SelectedIndex = bitrateIndex;
            else
                cmbBitrate.SelectedIndex = 0;

            if (isRegion)
                rbRegion.Checked = true;
            else
                rbFullscreen.Checked = true;

            currentRegion = region;
            UpdateRegionButtonText();
            UpdateRegionBorder();
        }

        private void SaveSettings()
        {
            ConfigManager.SaveSettings((int)numFps.Value, cmbBitrate.SelectedIndex, rbRegion.Checked, currentRegion);
        }

        private void UpdateRegionButtonText()
        {
            if (currentRegion.Width > 0 && currentRegion.Height > 0)
                btnSelectRegion.Text = $"範圍: {currentRegion.Width}x{currentRegion.Height}";
            else
                btnSelectRegion.Text = "選取範圍...";
        }

        private void LoadExistingFiles()
        {
            lstFiles.Items.Clear();

            var allFiles = new List<string>();
            
            // Load from all three folders
            if (Directory.Exists(VideoFolder))
                allFiles.AddRange(Directory.GetFiles(VideoFolder, "*.mp4"));
            
            if (Directory.Exists(ScreenshotFolder))
                allFiles.AddRange(Directory.GetFiles(ScreenshotFolder, "*.png"));
            
            if (Directory.Exists(AudioFolder))
                allFiles.AddRange(Directory.GetFiles(AudioFolder, "*.mp3"));

            var sortedFiles = allFiles
                .OrderByDescending(f => File.GetCreationTime(f))
                .ToArray();

            foreach (var file in sortedFiles)
            {
                lstFiles.Items.Add(new FileItem { FullPath = file });
            }
        }

        private void rbSource_CheckedChanged(object sender, EventArgs e)
        {
            btnSelectRegion.Enabled = rbRegion.Checked;
            UpdateOverlayPosition();
            UpdateRegionBorder();
        }

        private void chkShowKeys_CheckedChanged(object sender, EventArgs e)
        {
            UpdateKeyOverlayVisibility();
        }

        private void chkShowMouse_CheckedChanged(object sender, EventArgs e)
        {
            // Mouse visibility will be applied when starting recording
        }

        private void btnSelectRegion_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Hide border while selecting
            regionBorder?.Hide();
            
            using (var form = new SelectionForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    currentRegion = form.SelectedRegion;
                    UpdateRegionButtonText();
                    UpdateOverlayPosition();
                }
            }
            this.Show();
            UpdateRegionBorder();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
            KeyHandler.Unregister(this.Handle, HOTKEY_ID_START);
            KeyHandler.Unregister(this.Handle, HOTKEY_ID_SCREENSHOT);
            KeyHandler.Unregister(this.Handle, HOTKEY_ID_AUDIO);
            GlobalKeyboardHook.Stop();
            if (recorder.IsRecording)
            {
                recorder.StopRecording();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == KeyHandler.WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (id == HOTKEY_ID_START)
                {
                    // F9 toggles video recording
                    if (recorder.IsRecording)
                        StopRecording();
                    else
                        StartRecording();
                }
                else if (id == HOTKEY_ID_SCREENSHOT)
                {
                    // F10 takes screenshot
                    TakeScreenshot();
                }
                else if (id == HOTKEY_ID_AUDIO)
                {
                    // F8 toggles audio recording
                    if (recorder.IsRecording)
                        StopRecording();
                    else
                        StartAudioRecording();
                }
            }
            base.WndProc(ref m);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartRecording();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopRecording();
        }

        private RegionBorderForm regionBorder;

        private void UpdateRegionBorder()
        {
            if (rbRegion.Checked && currentRegion.Width > 0 && currentRegion.Height > 0)
            {
                if (regionBorder == null || regionBorder.IsDisposed)
                {
                    regionBorder = new RegionBorderForm();
                }
                
                // Ensure it's visible but not stealing focus if possible, though Show() usually takes focus.
                // We just want it to exist.
                if (!regionBorder.Visible) regionBorder.Show();
                
                regionBorder.Bounds = currentRegion;
                regionBorder.TopMost = true;
            }
            else
            {
                regionBorder?.Hide();
            }
        }

        private void StartRecording()
        {
            if (recorder.IsRecording) return;

            string fileName = Path.Combine(VideoFolder, $"Record_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            try
            {
                // Subscribe to status events if not already
                recorder.StatusChanged -= Recorder_StatusChanged;
                recorder.StatusChanged += Recorder_StatusChanged;

                int quality = 70; // Low
                switch (cmbBitrate.SelectedIndex)
                {
                    case 1: quality = 85; break; // Medium
                    case 2: quality = 98; break; // High
                }

                int fps = (int)numFps.Value;
                
                if (rbRegion.Checked && (currentRegion.Width == 0 || currentRegion.Height == 0))
                {
                    MessageBox.Show("請先選取錄影範圍", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hide border before recording
                regionBorder?.Hide();
                
                // Minimize window
                this.WindowState = FormWindowState.Minimized;

                recorder.StartRecording(fileName, fps, quality, rbRegion.Checked ? currentRegion : System.Drawing.Rectangle.Empty, chkShowMouse.Checked, chkAudio.Checked);
                
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                grpSettings.Enabled = false;
                UpdateStatus($"● 錄影中: {Path.GetFileName(fileName)}", true);
            }
            catch (Exception ex)
            {
                // Restore if failed
                this.WindowState = FormWindowState.Normal;
                UpdateRegionBorder();
                
                UpdateStatus($"錄製失敗: {ex.Message}");
                MessageBox.Show($"錄製失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Recorder_StatusChanged(object sender, string e)
        {
            UpdateStatus(e);
            
            // If recording completed, add to list
            if (e.StartsWith("Saved: "))
            {
                string filePath = e.Substring(7);
                if (File.Exists(filePath))
                {
                    AddFileToList(filePath);
                }
            }
        }

        private void AddFileToList(string filePath)
        {
            if (lstFiles.InvokeRequired)
            {
                lstFiles.Invoke(new Action<string>(AddFileToList), filePath);
            }
            else
            {
                lstFiles.Items.Insert(0, new FileItem { FullPath = filePath });
            }
        }

        private void lstFiles_DoubleClick(object sender, EventArgs e)
        {
            if (lstFiles.SelectedItem is FileItem fileItem)
            {
                if (File.Exists(fileItem.FullPath))
                {
                    try
                    {
                        Process.Start(fileItem.FullPath);
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"無法開啟檔案: {ex.Message}");
                    }
                }
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string folder = RecordFolder;
            try
            {
                // Create folder if it doesn't exist
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                    
                Process.Start("explorer.exe", folder);
            }
            catch (Exception ex)
            {
                UpdateStatus($"無法開啟資料夾: {ex.Message}");
            }
        }

        private void StopRecording()
        {
            if (!recorder.IsRecording) return;

            try
            {
                recorder.StopRecording();
                btnStart.Enabled = true;
                btnStop.Enabled = false;
                grpSettings.Enabled = true;
                UpdateStatus("正在儲存錄影...", false);
                
                // Restore window
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                    this.Activate();
                }
                
                // Show border again if needed
                UpdateRegionBorder();
            }
            catch (Exception ex)
            {
                UpdateStatus($"停止失敗: {ex.Message}", false);
            }
        }

        private void StartAudioRecording()
        {
            if (recorder.IsRecording) return;

            string fileName = Path.Combine(AudioFolder, $"Audio_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");
            try
            {
                // Subscribe to status events if not already
                recorder.StatusChanged -= Recorder_StatusChanged;
                recorder.StatusChanged += Recorder_StatusChanged;

                // Minimize window
                this.WindowState = FormWindowState.Minimized;

                recorder.StartAudioRecording(fileName);
                
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                grpSettings.Enabled = false;
                UpdateStatus($"● 錄音中: {Path.GetFileName(fileName)}", true);
            }
            catch (Exception ex)
            {
                // Restore if failed
                this.WindowState = FormWindowState.Normal;
                
                UpdateStatus($"錄音失敗: {ex.Message}");
                MessageBox.Show($"錄音失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TakeScreenshot()
        {
            try
            {
                Rectangle bounds;
                if (rbRegion.Checked && currentRegion.Width > 0 && currentRegion.Height > 0)
                {
                    bounds = currentRegion;
                }
                else
                {
                    bounds = Screen.PrimaryScreen.Bounds;
                }

                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    }

                    // Generate filename with milliseconds to avoid conflicts
                    string baseFileName = $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss_fff}";
                    string fileName = Path.Combine(ScreenshotFolder, baseFileName + ".png");
                    
                    // If file still exists (rare case), add a counter
                    int counter = 1;
                    while (File.Exists(fileName))
                    {
                        fileName = Path.Combine(ScreenshotFolder, $"{baseFileName}_{counter}.png");
                        counter++;
                    }

                    bitmap.Save(fileName, ImageFormat.Png);
                    AddFileToList(fileName);
                    
                    // Show brief confirmation without changing recording indicator
                    if (recorder.IsRecording)
                    {
                        UpdateStatus($"● 錄影中 - 截圖已儲存: {Path.GetFileName(fileName)}", true);
                    }
                    else
                    {
                        UpdateStatus($"截圖已儲存: {Path.GetFileName(fileName)}", false);
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"截圖失敗: {ex.Message}", recorder.IsRecording);
            }
        }

        private void UpdateStatus(string message, bool isRecording = false)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, bool>(UpdateStatus), message, isRecording);
            }
            else
            {
                lblStatus.Text = message;
                lblStatus.ForeColor = isRecording ? Color.Red : SystemColors.ControlText;
            }
        }
    }
    // Transparent form to show region border
    public class RegionBorderForm : Form
    {
        public RegionBorderForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.StartPosition = FormStartPosition.Manual;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Pen pen = new Pen(Color.Red, 3))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        // Pass clicks through
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTTRANSPARENT = -1;

            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
