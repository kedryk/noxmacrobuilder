using System;
using MacroBuilder.ImageCapture.Interface;

namespace MacroBuilder.ImageCapture.Hook
{
    internal interface IDXHook: IDisposable
    {
        CaptureInterface Interface
        {
            get;
            set;
        }
        CaptureConfig Config
        {
            get;
            set;
        }

        ScreenshotRequest Request
        {
            get;
            set;
        }

        void Hook();

        void Cleanup();
    }
}
