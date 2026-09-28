using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Adds the Stone Health total to the HP label. BaseLib draws the bands from StoneHealthPower's
/// forecast segments but never touches the text. This writes only text, so it composes with
/// BaseLib's colour postfix in either order.
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
