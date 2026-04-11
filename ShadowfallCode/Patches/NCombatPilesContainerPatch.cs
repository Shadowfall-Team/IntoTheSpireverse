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
    private static readonly string _scenePath =
        "res://" + MainFile.ModId + "/scenes/CargoPile.tscn";

    private static readonly string megaLabelFont =
        "res://themes/kreon_bold_glyph_space_one.tres";

    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    public static void ReadyPostfix(NCombatPilesContainer __instance)
    {
        var cargoPileButton = ResourceLoader.Load<PackedScene>(_scenePath)
            .Instantiate<NCargoPile>();
        var parentNode = cargoPileButton.GetNode<Control>("CountContainer");
        var background =
            cargoPileButton.GetNode<TextureRect>("CountContainer/Background");

        var countBg =
            ResourceLoader.Load<Texture2D>(
                "res://images/packed/combat_ui/pile_button_count.png");

        cargoPileButton.Name = "%CargoPile";
        cargoPileButton.Position = new Vector2(35, 700);

        background.Texture = countBg;

        var label = CreateCargoPileLabel();

        parentNode.AddChild(label);
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

    private static MegaLabel CreateCargoPileLabel()
    {
        var font = PreloadManager.Cache.GetAsset<Font>(megaLabelFont);

        var label = new MegaLabel();
        label.Name = "Count";
        label.LayoutMode = 1;
        label.AnchorsPreset = 15;
        label.OffsetLeft = 12;
        label.OffsetTop = -26;
        label.OffsetRight = -12;
        label.OffsetBottom = 26;
        label.GrowHorizontal = Control.GrowDirection.Both;
        label.GrowVertical = Control.GrowDirection.Both;
        label.HorizontalAlignment = HorizontalAlignment.Center;
        label.VerticalAlignment = VerticalAlignment.Center;
        label.AddThemeFontOverride(ThemeConstants.Label.font, font);
        label.AddThemeColorOverride(ThemeConstants.Label.fontColor,
            new Color(1f, 0.96f, 0.88f));
        label.AddThemeColorOverride(ThemeConstants.Label.fontOutlineColor,
            new Color(0.53f, 0.12f, 0.12f));
        label.AddThemeConstantOverride(ThemeConstants.Label.outlineSize, 12);
        label.AddThemeConstantOverride(ThemeConstants.Label.fontSize, 26);
        label.MinFontSize = 20;
        label.MaxFontSize = 26;
        return label;
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