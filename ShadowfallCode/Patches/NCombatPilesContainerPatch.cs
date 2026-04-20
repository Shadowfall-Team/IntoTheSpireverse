using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Shadowfall.ShadowfallCode.ui;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(NCombatPilesContainer))]
public static class NCombatPilesContainerPatch
{
    private static readonly string _scenePath = "res://Shadowfall/scenes/CargoPile.tscn";

    private static readonly string megaLabelFont = "res://themes/kreon_bold_glyph_space_one.tres";

    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    public static void ReadyPostfix(NCombatPilesContainer __instance)
    {
        var cargoPileButton = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NCargoPile>();
        cargoPileButton.Name = "%CargoPile";
        cargoPileButton.Position = new Vector2(35, 700);
        
        var countLabel = cargoPileButton.GetNode<ShadowfallMegaLabel>("CountContainer/Count");
        var font = PreloadManager.Cache.GetAsset<Font>(megaLabelFont);
        countLabel.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        countLabel.MinFontSize = 20;
        countLabel.MaxFontSize = 26;
        
        var background = cargoPileButton.GetNode<TextureRect>("CountContainer/Background");
        var countBg = ResourceLoader.Load<Texture2D>("res://images/packed/combat_ui/pile_button_count.png");
        background.Texture = countBg;

        __instance.AddChild(cargoPileButton);
    }

    [HarmonyPatch("Enable")]
    [HarmonyPostfix]
    public static void EnablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCargoPile>("_CargoPile")?.Enable();
    }

    [HarmonyPatch("Disable")]
    [HarmonyPostfix]
    public static void DisablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCargoPile>("_CargoPile")?.Disable();
    }
}

[HarmonyPatch(typeof(NCombatUi), "Activate")]
public static class NCombatUiActivatePatch
{
    [HarmonyPostfix]
    public static void ActivatePostfix(NCombatUi __instance, CombatState state)
    {
        var container = __instance.GetNode<NCombatPilesContainer>("%CombatPileContainer");
        var cargoPile = container.GetNodeOrNull<NCargoPile>("_CargoPile");
        var player = LocalContext.GetMe(state);
        cargoPile?.Initialize(player);
    }
}