using System.Collections.Generic;

namespace MacroBuilder.ImageCapture.Hook.Common
{
    public interface IOverlay: IOverlayElement
    {
        List<IOverlayElement> Elements { get; set; }
    }
}
