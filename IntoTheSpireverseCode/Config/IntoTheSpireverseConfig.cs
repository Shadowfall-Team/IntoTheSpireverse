using BaseLib.Config;
using BaseLib.Config.UI;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches.Input;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Config;

[ConfigHoverTipsByDefault]
internal class IntoTheSpireverseConfig : SimpleModConfig
{
    [ConfigSection("ShadowRegent")]
    public static bool ShowCargoCardStack { get; set; } = true;
    public static bool ShowAmmoReminder { get; set; } = false;

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

    [ConfigHideInUI] public static string SelectedAltIronclad { get; set; } = nameof(Ironclad);
    [ConfigHideInUI] public static string SelectedAltSilent { get; set; } = nameof(Silent);
    [ConfigHideInUI] public static string SelectedAltNecrobinder { get; set; } = nameof(Necrobinder);
    [ConfigHideInUI] public static string SelectedAltRegent { get; set; } = nameof(Regent);
    [ConfigHideInUI] public static string SelectedAltDefect { get; set; } = nameof(Defect);

    public static string GetSelectedAlt(string baseName) => baseName switch
    {
        nameof(Ironclad) => SelectedAltIronclad,
        nameof(Silent) => SelectedAltSilent,
        nameof(Necrobinder) => SelectedAltNecrobinder,
        nameof(Regent) => SelectedAltRegent,
        nameof(Defect) => SelectedAltDefect,
        _ => baseName
    };

    public static void SaveSelectedAlt(CharacterModel character)
    {
        var baseName = (character is IAltCharacter alt ? alt.BaseCharacterModel : character).GetType().Name;
        var skinName = character.GetType().Name;

        switch (baseName)
        {
            case nameof(Ironclad): SelectedAltIronclad = skinName; break;
            case nameof(Silent): SelectedAltSilent = skinName; break;
            case nameof(Necrobinder): SelectedAltNecrobinder = skinName; break;
            case nameof(Regent): SelectedAltRegent = skinName; break;
            case nameof(Defect): SelectedAltDefect = skinName; break;
            default: return;
        }

        SaveDebounced<IntoTheSpireverseConfig>();
    }
}
