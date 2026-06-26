using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Enchantments;
using IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

//TODO: i'm not a fan of cross-patch information (RockWasTransformedFrom). It makes some very bold assumptions.
// but I cannot think of a better implementation. Let's take another look at this some other time.
[HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterTransformedFrom))]
public static class RockTransformFromPatch
{
    public static bool RockWasTransformedFrom;

    public static void Postfix(CardModel __instance)
    {
        RockWasTransformedFrom = __instance.Tags.Contains(IntoTheSpireverseCardTags.Rock);
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterTransformedTo))]
public static class RockTransformToPatch
{
    public static void Postfix(CardModel __instance)
    {
        if (!RockTransformFromPatch.RockWasTransformedFrom) return;
        RockTransformFromPatch.RockWasTransformedFrom = false;

        if (!__instance.Tags.Contains(IntoTheSpireverseCardTags.Rock)) return;

        var relic = __instance.Owner.Relics.OfType<MudIdol>().FirstOrDefault();
        if (relic == null) return;

        relic.Flash();
        CardCmd.Upgrade(__instance);
        CardCmd.Enchant<Polished>(__instance, 1m);
    }
}