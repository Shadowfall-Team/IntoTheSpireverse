using Godot;
using MegaCrit.Sts2.addons.mega_text;

namespace IntoTheSpireverse.IntoTheSpireverseCode.ui;

public partial class NAmmoCounter : Control
{
    private IntoTheSpireverseMegaLabel? _countLabel;

    private IntoTheSpireverseMegaLabel CountLabel =>
        _countLabel ??= GetNode<IntoTheSpireverseMegaLabel>("%Count");

    public void SetCount(int count)
    {
        CountLabel.Text = count.ToString();
    }

    public void ApplyFont(Font font, int minSize, int maxSize)
    {
        CountLabel.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        CountLabel.MinFontSize = minSize;
        CountLabel.MaxFontSize = maxSize;
    }
}
