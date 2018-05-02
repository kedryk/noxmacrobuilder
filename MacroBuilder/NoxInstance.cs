using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using MacroBuilder.ImageCapture;
using MacroBuilder.ImageCapture.Hook;
using MacroBuilder.ImageCapture.Interface;
using MacroBuilder.Pinvoke;

namespace MacroBuilder {
    public class NoxInstance {
        private readonly Process _process;
        private CaptureProcess _captureProcess;
        private int? _phoneScreenHandle;
        private AutomationElement _phoneScreen;
        private Point _offset = new Point(0,0);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hDC, int flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }


        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        private static IntPtr MakeLParam(int LoWord, int HiWord) {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        public string Name => _process?.MainWindowTitle;

        public NoxInstance(Process process) {
            _process = process;
            // We want the next to largest child window of this process.
            var mainWindow = AutomationElement.FromHandle(_process.MainWindowHandle);
            _phoneScreen = mainWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "sub"));
            _phoneScreenHandle = _phoneScreen?.Current.NativeWindowHandle;

            _offset = new Point((int)(_phoneScreen.Current.BoundingRectangle.Left - mainWindow.Current.BoundingRectangle.Left), (int)(_phoneScreen.Current.BoundingRectangle.Top - mainWindow.Current.BoundingRectangle.Top));
        }

        public void Click(int x, int y) {
            var handle = _process.MainWindowHandle;
            var position = MakeLParam(x + _offset.X, y + _offset.Y);
            PostMessage(handle, (uint)WindowMessage.LBUTTONDOWN, 0x0001, position);
            PostMessage(handle, (uint)WindowMessage.LBUTTONUP, 0x0000, position);
        }

        public void TouchDown(int x, int y) {
            if (_phoneScreenHandle == null) { return; }
            var handle = _process.MainWindowHandle;
            var position = MakeLParam(x + _offset.X, y + _offset.Y);
            PostMessage(handle, (uint)WindowMessage.LBUTTONDOWN, 0x0001, position);
        }
        public void TouchMove(int x, int y) {
            if (_phoneScreenHandle == null) { return; }
            var handle = _process.MainWindowHandle;
            var position = MakeLParam(x + _offset.X, y + _offset.Y);
            PostMessage(handle, (uint)WindowMessage.MOUSEMOVE, 0x0001, position);
        }
        public void TouchUp(int x, int y) {
            if (_phoneScreenHandle == null) { return; }
            var handle = _process.MainWindowHandle;
            var position = MakeLParam(x + _offset.X, y + _offset.Y);
            PostMessage(handle, (uint)WindowMessage.LBUTTONUP, 0x0000, position);
        }

        public Image TakeScreenshot() {
            return _captureProcess.CaptureInterface.GetScreenshot(new Rectangle(0, 0, 0, 0), new TimeSpan(0, 0, 2), null, ImageFormat.Bitmap).ToBitmap();
        }

        public override string ToString() => $"[{_process.Id}] - {Name}";

        public void AddHooks() {
            if (HookManager.IsHooked(_process.Id)) { return; }

            var captureConfig = new CaptureConfig {
                Direct3DVersion = Direct3DVersion.AutoDetect,
                ShowOverlay = true
            };
            var captureInterface = new CaptureInterface();
            captureInterface.RemoteMessage += CaptureInterfaceOnRemoteMessage;
            _captureProcess = new CaptureProcess(_process, captureConfig, captureInterface);
        }

        private void CaptureInterfaceOnRemoteMessage(MessageReceivedEventArgs message) {
            
        }

        public void RemoveHooks() => HookManager.RemoveHookedProcess(_process.Id);
    }
}
