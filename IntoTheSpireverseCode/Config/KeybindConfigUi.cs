using BaseLib.Config.UI;
using Godot;
using MegaCrit.Sts2.Core.Localization;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Config;

internal static class KeybindConfigUi
{
    public static string BindingLabel(Key modifier, Key key)
    {
        var modStr = modifier == Key.None ? "" : modifier + " + ";
        return modStr + key;
    }

    public static void SetButtonLabel(Control optionContainer, string rowName, string text)
    {
        var row = optionContainer.GetNodeOrNull<NConfigOptionRow>("%" + rowName);
        var label = (row?.SettingControl as NConfigButton)?.GetNodeOrNull<Label>("Label");
        label?.Text = text;
    }

    public static void SetListening(NConfigButton button) =>
        button.GetNodeOrNull<Label>("Label")!.Text =
            new LocString("settings_ui", "INTOTHESPIREVERSE-PRESS_A_KEY").GetFormattedText();
}
