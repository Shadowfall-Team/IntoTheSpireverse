using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Patches;
[HarmonyPatch(typeof(RelicModel), "get_Description")]
public static class HeartRelicDescriptionPatch
{
    public static bool Prefix(RelicModel __instance, ref LocString __result)
    {
        if (__instance is not HeartOfStone and not HeartOfTheMountain) return true;
        if (!__instance.IsMutable) return true;
        if (__instance.Owner?.Creature == null) return true;

        var entry = __instance is HeartOfStone
            ? "SHADOWFALL-HEART_OF_STONE.descriptionInRun"
            : "SHADOWFALL-HEART_OF_THE_MOUNTAIN.descriptionInRun";

        var healAmount = (int)(__instance.Owner.Creature.MaxHp * __instance.DynamicVars["HealPercent"].BaseValue / 100m);
        var loc = new LocString("relics", entry);
        loc.Add(__instance.DynamicVars["HealPercent"]);
        loc.Add(__instance.DynamicVars["MaxHp"]);
        loc.Add("HealPreview", healAmount.ToString());
        __result = loc;
        return false;
    }
}