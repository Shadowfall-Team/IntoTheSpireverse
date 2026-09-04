using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Enchantments;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

public class RockTransformPatches
{
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterTransformedFrom))]
    public static class RockTransformFromPatch
    {
        public static bool StatusWasTransformedFrom;

        public static void Postfix(CardModel __instance)
        {
            StatusWasTransformedFrom = __instance.Type == CardType.Status;
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterTransformedTo))]
    public static class RockTransformToPatch
    {
        public static void Postfix(CardModel __instance)
        {
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