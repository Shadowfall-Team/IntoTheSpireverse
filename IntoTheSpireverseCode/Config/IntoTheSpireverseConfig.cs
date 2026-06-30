using BaseLib.Config;
using BaseLib.Config.UI;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches.Input;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Config;

[ConfigHoverTipsByDefault]
internal class IntoTheSpireverseConfig : SimpleModConfig
{
    [ConfigSection("ShadowRegent")]
    public static bool ShowCargoCardStack { get; set; } = true;

    /*
    [ConfigSection("Development")]
    public static bool ShowWipContent { get; set; } = false;
    */

    [ConfigSection("Keybinds")]

    [ConfigHideInUI]
    public static Key CargoPileModifier { get; set; } = Key.Ctrl;

    [ConfigHideInUI]
    public static Key CargoPileKey { get; set; } = Key.A;

    [ConfigButton("CaptureCargoPileBinding")]
    [ConfigHoverTip]
    public static void CaptureCargoPileBinding(NConfigButton button)
    {
        NInputManagerPatches.StartCapture(NInputManagerPatches.CaptureTarget.CargoPile, button);
        KeybindConfigUi.SetListening(button);
    }

    [ConfigHideInUI]
    public static Key FireModifier { get; set; } = Key.Ctrl;

    [ConfigHideInUI]
    public static Key FireKey { get; set; } = Key.F;

    [ConfigButton("CaptureFireBinding")]
    [ConfigHoverTip]
    public static void CaptureFireBinding(NConfigButton button)
    {
        NInputManagerPatches.StartCapture(NInputManagerPatches.CaptureTarget.Fire, button);
        KeybindConfigUi.SetListening(button);
    }

    public override void SetupConfigUI(Control optionContainer)
    {
        base.SetupConfigUI(optionContainer);
        KeybindConfigUi.SetButtonLabel(optionContainer, "CaptureCargoPileBinding", KeybindConfigUi.BindingLabel(CargoPileModifier, CargoPileKey));
        KeybindConfigUi.SetButtonLabel(optionContainer, "CaptureFireBinding", KeybindConfigUi.BindingLabel(FireModifier, FireKey));
    }
}
