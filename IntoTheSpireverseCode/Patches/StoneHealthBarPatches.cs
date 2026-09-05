using Godot;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Draws Stone Health onto the health bar, after the red band and before the empty remainder.
///
/// The base game authors PoisonForeground and DoomForeground into the creature scene, which lives in
/// the game's own pck and cannot be edited from a mod, so the two extra bands are cloned from
/// HpForeground at runtime and looked up by name rather than cached - that keeps them correct across
/// the bar being re-laid-out or the node being freed.
///
/// Every band is sized through the same units-to-pixels conversion the red band uses, so one pixel
/// means one HP in all four colours.
/// </summary>
[HarmonyPatch]
public static class StoneHealthBarPatches
{
    private const string GreyBandName = "ITS_StoneHealth";
    private const string WhiteBandName = "ITS_StoneHealthOverflow";

    private static readonly Color GreyColor = new("A8A8A8");
    private static readonly Color WhiteColor = new("FFFFFF");

    private static int StoneOf(Creature? creature) =>
        creature == null ? 0 : creature.GetPowerAmount<StoneHealthPower>();

    private static Control? Band(NHealthBar bar, string name, Color color)
    {
        var template = bar._hpForeground;
        var parent = template?.GetParent();
        if (template == null || parent == null) return null;

        if (parent.GetNodeOrNull<Control>(name) is { } existing) return existing;

        if (template.Duplicate((int)Node.DuplicateFlags.Signals) is not Control band) return null;
        band.Name = name;
        band.UniqueNameInOwner = false;
        band.SelfModulate = color;
        band.Visible = false;
        parent.AddChild(band);
        return band;
    }

    [HarmonyPatch(typeof(NHealthBar), "RefreshForeground")]
    public static class Foreground
    {
        static void Postfix(NHealthBar __instance)
        {
            if (!IntoTheSpireverseConfig.ShowStoneHealthOnBar) return;

            var grey = Band(__instance, GreyBandName, GreyColor);
            var white = Band(__instance, WhiteBandName, WhiteColor);
            if (grey == null || white == null) return;

            var creature = __instance._creature;
            var stone = StoneOf(creature);
            var maxFg = __instance.MaxFgWidth;

            // Hide while dead, with no pool, or when a lethal Poison/Doom overlay has taken over the
            // red band - those states repaint the whole bar and Stone has no meaningful place in them.
            if (creature == null || creature.CurrentHp <= 0 || stone <= 0
                || __instance._hpForeground is not { Visible: true }
                || creature.MaxHp <= 0 || maxFg <= 0f)
            {
                grey.Visible = false;
                white.Visible = false;
                return;
            }

            var maxHp = creature.MaxHp;
            var overflow = Math.Max(0, creature.CurrentHp + stone - maxHp);

            float Width(int units) => units / (float)maxHp * maxFg;

            // White always occupies the rightmost `overflow` units of the bar, painting over whatever
            // is beneath it - grey first, then red once grey is exhausted. At full HP that means it
            // eats into red, which is the only way a full-HP character with Stone shows anything.
            var whiteWidth = Math.Min(Width(overflow), maxFg);
            var whiteStart = maxFg - whiteWidth;

            // Grey runs from where red ends up to where white begins. GetFgWidth floors a living
            // creature's bar at 12px, so mirror that or grey would draw underneath red.
            var greyStart = Math.Max(Width(creature.CurrentHp), 12f);
            var greyEnd = Math.Min(greyStart + Width(stone), whiteStart);

            Place(grey, greyStart, greyEnd, maxFg, greyEnd > greyStart);
            Place(white, whiteStart, maxFg, maxFg, whiteWidth > 0f);
        }

        /// <summary>
        /// Bands are NinePatchRects with rounded end caps. A band that begins exactly where the
        /// previous one ends leaves the previous band's cap visible, so the red bar reads as tapering
        /// off before the grey starts. Poison solves this by starting PatchMarginLeft pixels early so
        /// its own left cap covers the cap beneath; do the same. Only the left edge moves, so the
        /// boundary between bands still falls on the correct value.
        /// </summary>
        private static void Place(Control band, float left, float right, float maxFg, bool visible)
        {
            band.Visible = visible;
            if (!visible) return;

            var capOverlap = band is NinePatchRect patch ? patch.PatchMarginLeft : 0;
            band.OffsetLeft = Math.Max(0f, left - capOverlap);
            band.OffsetRight = Math.Min(right, maxFg) - maxFg;
        }
    }

    [HarmonyPatch(typeof(NHealthBar), "RefreshText")]
    public static class Text
    {
        static void Postfix(NHealthBar __instance)
        {
            if (!IntoTheSpireverseConfig.ShowStoneHealthOnBar) return;

            var creature = __instance._creature;
            if (creature == null || creature.CurrentHp <= 0) return;
            if (!creature.HpDisplay.ShowsNumbers()) return;

            var stone = StoneOf(creature);
            if (stone <= 0) return;

            // The parenthetical is deliberately uncapped: 40 HP with 80 Stone against a max of 80
            // reads 40/80 (120), and the white band shows how far past the bar that overshoot goes.
            __instance._hpLabel.SetTextAutoSize(
                $"{creature.CurrentHp}/{creature.MaxHp} ({creature.CurrentHp + stone})");
        }
    }
}
