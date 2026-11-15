using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Enchantments;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Rocks;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// v113 changed the method names, and they now include the new card, or the previous card respectively
/// AfterThisWasTransformedFrom is overridden on <see cref="RockCardBase"/>, which should handle it without any patches.
/// The TargetMethod methods still return the new method (The names were swapped between versions,
/// so "Into" is now called where the "From" method was befor, and vice versa) so the game doesn't error
/// on load (until the name/signiature changes again)
/// </summary>
[HarmonyPatch]
public static class RockTransformPatches
{
    [HarmonyPatch(typeof(CardModel))]
    public static class RockTransformFromPatch
    {
        [HarmonyTargetMethod]
        static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(CardModel), "AfterTransformedFrom")
                ?? AccessTools.Method(typeof(CardModel), nameof(CardModel.AfterThisWasTransformedInto), [typeof(CardModel)])
                ?? throw new MethodAccessException("Neither CardModel.AfterTransformedFrom or a known CardModel.AfterThisWasTransformedFrom signature found!");
        }

        public static bool StatusWasTransformedFrom;

        public static void Postfix(CardModel __instance, object[] __args)
        {
            if (__args.Length != 0) { return; }

            StatusWasTransformedFrom = __instance.Type == CardType.Status;
        }
    }

    [HarmonyPatch(typeof(CardModel))]
    public static class RockTransformToPatch
    {
        [HarmonyTargetMethod]
        static MethodInfo TargetMethod()
        {
            return AccessTools.Method(typeof(CardModel), "AfterTransformedTo")
                ?? AccessTools.Method(typeof(CardModel), nameof(CardModel.AfterThisWasTransformedFrom), [typeof(CardModel)])
                ?? throw new MethodAccessException("Neither CardModel.AfterTransformedTo or a known CardModel.AfterThisWasTransformedInto signature found!");
        }

        public static void Postfix(CardModel __instance, object[] __args)
        {
            if (__args.Length != 0) { return; }

            if (!RockTransformFromPatch.StatusWasTransformedFrom) return;
            RockTransformFromPatch.StatusWasTransformedFrom = false;

            if (!__instance.Tags.Contains(IntoTheSpireverseCardTags.Rock)) return;

            var relic = __instance.Owner?.Relics.OfType<MudIdol>().FirstOrDefault();
            if (relic == null) return;

            relic.Flash();
            CardCmd.Upgrade(__instance);
            CardCmd.Enchant<Polished>(__instance, 1m);
        }
    }
}