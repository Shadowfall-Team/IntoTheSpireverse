using Godot;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Shows a gear flag on the card frame while the card carries a Modification, reusing the
/// base game's enchantment flag artwork.
///
/// The flag is a duplicate of the card scene's %Enchantment tab rather than a hand-built node,
/// so it inherits that flag's shape, size and material, and follows along if the base game
/// restyles them. Only the hue and the icon are overridden, to distinguish it from a second
/// enchantment. UpdateVisuals is the hook because it is the general card refresh - it is what
/// calls UpdateEnchantmentVisuals - so the flag stays correct in hand, deck views, and previews.
/// </summary>
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
public static class NCardModificationFlagPatch
{
    private const string FlagName = "ModificationFlag";

    // Vertical gap between the enchantment flag and the gear flag when both are shown.
    private const float SlotGap = 4f;

    // The flag's colour does not come from its texture - the card scene puts a ShaderMaterial
    // (res://shaders/hsv.gdshader) on the enchantment tab, and NCard.SetEnchantmentStatus
    // drives it with h/s/v. Normal status is h=0.25, s=0.4, v=0.6, which is the subtle dark
    // blue. Overriding h alone therefore recolours the flag while keeping that exact tone,
    // and needs no new art.
    //
    // The shader's hue is offset from a plain 0-1 wheel, measured from two known points:
    // h=0.25 renders blue (wheel 0.667) and h=0.333 renders purple (wheel 0.75), so the shader
    // shows hue h+0.417. Green is 0.333 on the wheel, giving h = 0.333 - 0.417 = -0.084, which
    // wraps to 0.917. Equivalently: green is a third of the wheel below blue, so count down
    // from the game's 0.25 rather than up.
    private const float BackgroundHue = 0.917f;

    // The s/v half of that same normal-status triple.
    private const float EnabledSaturation = 0.4f;
    private const float EnabledValue = 0.6f;

    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        if (!__instance.IsNodeReady() || __instance.Model == null) return;

        var enchantmentTab = __instance.EnchantmentTab;
        var parent = enchantmentTab?.GetParent();
        if (enchantmentTab == null || parent == null) return;

        var flag = parent.GetNodeOrNull<Control>(FlagName);

        if (!Modification.IsModified(__instance.Model))
        {
            if (flag != null) flag.Visible = false;
            return;
        }

        flag ??= CreateFlag(enchantmentTab, parent);
        if (flag == null) return;

        flag.Visible = true;

        // Take the enchantment's own slot when there is no enchantment, drop to the slot
        // below when there is one.
        var offsetY = enchantmentTab.Visible ? enchantmentTab.Size.Y + SlotGap : 0f;
        flag.Position = enchantmentTab.Position + new Vector2(0f, offsetY);
    }

    private static Control? CreateFlag(Control enchantmentTab, Node parent)
    {
        // Duplicate without Signals: the source tab's connections belong to the enchantment,
        // and this flag is inert - it must not react to anything on the enchantment's behalf.
        const int flags = (int)(Node.DuplicateFlags.Groups
                                | Node.DuplicateFlags.Scripts
                                | Node.DuplicateFlags.UseInstantiation);

        if (enchantmentTab.Duplicate(flags) is not Control flag) return null;

        flag.Name = FlagName;
        // The source is addressed as %Enchantment; a second node claiming that unique name
        // would collide with it.
        flag.UniqueNameInOwner = false;

        parent.AddChild(flag);

        // "Just the flag plus a gear icon" - no amount label.
        if (flag.GetNodeOrNull<Control>("Label") is { } label) label.Visible = false;
        if (flag.GetNodeOrNull<TextureRect>("Icon") is { } icon)
            icon.Texture = ResourceLoader.Load<Texture2D>(Modification.IconPath);

        RecolourFlag(flag);

        // Purely decorative. The Modify keyword tip reaches the player through the card's own
        // HoverTips (see Modification.AddTips), exactly as enchantment tips do - the flag
        // itself is never hovered, so it must not intercept input meant for the card.
        flag.MouseFilter = Control.MouseFilterEnum.Ignore;

        return flag;
    }

    /// <summary>
    /// Recolours the borrowed flag so it reads as its own thing rather than a second
    /// enchantment, by retuning the hsv material the card scene already put on it rather than
    /// attaching one of our own.
    /// </summary>
    private static void RecolourFlag(Control flag)
    {
        if (flag.Material is not ShaderMaterial inherited) return;

        // Duplicate first. Node.Duplicate copies the material by reference, so the flag is
        // sharing the live enchantment tab's material - writing h to it would recolour every
        // real enchantment flag in the game too.
        var material = (ShaderMaterial)inherited.Duplicate();
        material.SetShaderParameter("h", BackgroundHue);

        // Pin s/v to the same values SetEnchantmentStatus uses for an enabled enchantment,
        // rather than inheriting whatever the source tab happened to hold. On a card with no
        // enchantment the game never calls SetEnchantmentStatus, so the tab still carries the
        // scene's defaults - without this, flags would look different depending on whether the
        // host card happens to be enchanted.
        material.SetShaderParameter("s", EnabledSaturation);
        material.SetShaderParameter("v", EnabledValue);

        flag.Material = material;

        // SetEnchantmentStatus also tints the tab via Modulate; ours is not an enchantment, so
        // pin it to the value the enabled state uses rather than inheriting a stale one.
        flag.Modulate = Colors.White;
    }
}
