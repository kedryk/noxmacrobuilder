using System;

namespace MacroBuilder.ImageCapture.Hook.Common
{
    public interface IOverlayElement : ICloneable
    {
        bool Hidden { get; set; }

        void Frame();
    }
}
