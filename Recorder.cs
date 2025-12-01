using System;
using System.IO;
using ScreenRecorderLib;

namespace ScreenRecorder
{
    public class Recorder
    {
        private ScreenRecorderLib.Recorder _recorder;
        private string _currentFileName;

        public bool IsRecording { get; private set; }

        public event EventHandler<string> StatusChanged;

        public Recorder()
        {
            // Initialize is done in StartRecording to allow fresh options each time
        }

        public void StartRecording(string fileName, int fps, int quality, System.Drawing.Rectangle region, bool showMouse, bool audioEnabled)
        {
            if (IsRecording) return;

            _currentFileName = fileName;

            var options = new RecorderOptions
            {
                // Ensure the output folder exists
                OutputOptions = new OutputOptions
                {
                    RecorderMode = RecorderMode.Video,
                    SourceRect = region != System.Drawing.Rectangle.Empty ? new ScreenRect(region.Left, region.Top, region.Width, region.Height) : null
                },
                VideoEncoderOptions = new VideoEncoderOptions
                {
                    Quality = quality,
                    Framerate = fps,
                    IsFixedFramerate = true,
                    IsHardwareEncodingEnabled = false,
                    IsFragmentedMp4Enabled = true,  // Enable Fragmented MP4 for crash protection
                    Encoder = new H264VideoEncoder
                    {
                        BitrateMode = H264BitrateControlMode.Quality,
                        EncoderProfile = H264Profile.High,
                    }
                },
                MouseOptions = new MouseOptions
                {
                    IsMousePointerEnabled = showMouse,
                    IsMouseClicksDetected = showMouse
                },
                AudioOptions = new AudioOptions
                {
                    IsAudioEnabled = audioEnabled,
                    IsInputDeviceEnabled = audioEnabled,
                    IsOutputDeviceEnabled = audioEnabled,
                }
            };

            _recorder = ScreenRecorderLib.Recorder.CreateRecorder(options);
            _recorder.OnRecordingComplete += Recorder_OnRecordingComplete;
            _recorder.OnRecordingFailed += Recorder_OnRecordingFailed;
            _recorder.OnStatusChanged += Recorder_OnStatusChanged;

            _recorder.Record(_currentFileName);
            IsRecording = true;
        }

        public void StartAudioRecording(string fileName)
        {
            if (IsRecording) return;

            _currentFileName = fileName;

            var options = new RecorderOptions
            {
                OutputOptions = new OutputOptions
                {
                    RecorderMode = RecorderMode.Video  // Use Video mode but with only audio enabled
                },
                VideoEncoderOptions = new VideoEncoderOptions
                {
                    IsFragmentedMp4Enabled = true,   // Enable Fragmented MP4 for crash protection
                    IsThrottlingDisabled = false,
                    IsHardwareEncodingEnabled = false,
                    IsLowLatencyEnabled = false,
                    Quality = 70
                },
                AudioOptions = new AudioOptions
                {
                    IsAudioEnabled = true,
                    IsInputDeviceEnabled = true,
                    IsOutputDeviceEnabled = true,
                }
            };

            _recorder = ScreenRecorderLib.Recorder.CreateRecorder(options);
            _recorder.OnRecordingComplete += Recorder_OnRecordingComplete;
            _recorder.OnRecordingFailed += Recorder_OnRecordingFailed;
            _recorder.OnStatusChanged += Recorder_OnStatusChanged;

            _recorder.Record(_currentFileName);
            IsRecording = true;
        }

        public void StopRecording()
        {
            if (!IsRecording || _recorder == null) return;

            _recorder.Stop();
            IsRecording = false;
        }

        private void Recorder_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            StatusChanged?.Invoke(this, $"Status: {e.Status}");
        }

        private void Recorder_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            IsRecording = false;
            StatusChanged?.Invoke(this, $"Failed: {e.Error}");
        }

        private void Recorder_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            IsRecording = false;
            StatusChanged?.Invoke(this, $"Saved: {e.FilePath}");
            _recorder?.Dispose();
            _recorder = null;
        }
    }
}
