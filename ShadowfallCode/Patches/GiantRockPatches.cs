using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.CardTags;

namespace Shadowfall.ShadowfallCode.Patches;

public static class GiantRockBuffTracker
{
    private static readonly ConditionalWeakTable<GiantRock, StrongBox<decimal>> ExtraDamage = new();

    public static decimal Get(GiantRock card) =>
        ExtraDamage.GetValue(card, _ => new StrongBox<decimal>(0m)).Value;

    public static void Add(GiantRock card, decimal amount)
    {
        var box = ExtraDamage.GetValue(card, _ => new StrongBox<decimal>(0m));
        box.Value += amount;
        card.DynamicVars.Damage.BaseValue += amount;
    }
}

[HarmonyPatch(typeof(CardModel), "CanonicalTags", MethodType.Getter)]
public static class GiantRockPatches
{
    public static void Postfix(CardModel __instance, ref HashSet<CardTag> __result)
    {
        if (__instance is GiantRock)
            __result.Add(ShadowfallCardTags.Rock);
    }
}

[HarmonyPatch(typeof(CardModel), "AfterDowngraded")]
public static class GiantRockDowngradePatch
{
    public static void Postfix(CardModel __instance)
    {
        if (__instance is GiantRock giant)
            giant.DynamicVars.Damage.BaseValue += GiantRockBuffTracker.Get(giant);
    }
}