using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Adds the Stone Health total to the health bar's HP label.
///
/// The bands themselves are drawn by BaseLib from the segments StoneHealthPower reports through
/// IHealthBarForecastSource - see StoneHealthPower.GetHealthBarForecastSegments. Only the label is
/// left here, because the forecast API renders bars and does not touch the text.
///
/// This writes the label's text while BaseLib's own RefreshText postfix writes its colour theme
/// overrides, so the two compose regardless of which Harmony postfix runs first.
/// </summary>
[HarmonyPatch]
public static class StoneHealthBarPatches
{
    [HarmonyPatch(typeof(NHealthBar), "RefreshText")]
    public static class Text
    {
        static void Postfix(NHealthBar __instance)
        {
            if (!IntoTheSpireverseConfig.ShowStoneHealthOnBar) return;

            var creature = __instance._creature;
            if (creature == null || creature.CurrentHp <= 0) return;
            if (!creature.HpDisplay.ShowsNumbers()) return;

            var stone = creature.GetPowerAmount<StoneHealthPower>();
            if (stone <= 0) return;

            // The parenthetical is deliberately uncapped: 40 HP with 80 Stone against a max of 80
            // reads 40/80 (120), and the white band shows how far past the bar that overshoot goes.
            __instance._hpLabel.SetTextAutoSize(
                $"{creature.CurrentHp}/{creature.MaxHp} ({creature.CurrentHp + stone})");
        }
    }
}
