using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Shows Gabbro's bonus in Potion-Shaped Rock's tooltip. The belt and the potion popup both rebuild
/// the text from DynamicDescription on every hover, so it tracks Gabbro without any refresh.
///
/// Canonical potions are skipped: they have no owner, and HoverTipFactory.FromPotion caches their
/// tip, which is why Alkalize's preview keeps showing the base damage.
/// </summary>
[HarmonyPatch(typeof(PotionModel), nameof(PotionModel.DynamicDescription), MethodType.Getter)]
public static class PotionShapedRockTooltipPatch
{
    static void Postfix(PotionModel __instance, LocString __result)
    {
        var bonus = GabbroPower.PotionRockBonus(__instance);
        if (bonus == 0m) return;

        __result.Add("Damage", __instance.DynamicVars.Damage.BaseValue + bonus);
    }
}
