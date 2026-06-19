using Godot;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Shadowfall.ShadowfallCode.ui;

public class ComboControllerIcons
{
    private readonly TextureRect _iconA;
    private readonly TextureRect _iconB;
    private readonly CanvasItem? _separator;
    private readonly string _actionA;
    private readonly string _actionB;

    public ComboControllerIcons(TextureRect iconA, TextureRect iconB, string actionA, string actionB, CanvasItem? separator = null)
    {
        _iconA = iconA;
        _iconB = iconB;
        _separator = separator;
        _actionA = actionA;
        _actionB = actionB;
    }

    public void Refresh(bool enabled = true)
    {
        var cm = NControllerManager.Instance;
        var im = NInputManager.Instance;
        if (cm == null || im == null) return;

        var show = cm.IsUsingController && enabled;
        _iconA.Visible = show;
        _iconB.Visible = show;
        _separator?.Visible = show;

        if (!show) return;

        var texA = im.GetHotkeyIcon(_actionA);
        if (texA != null) _iconA.Texture = texA;

        var texB = im.GetHotkeyIcon(_actionB);
        if (texB != null) _iconB.Texture = texB;
    }
}
