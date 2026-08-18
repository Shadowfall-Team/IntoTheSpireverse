using System.Reflection;
using BaseLib.Utils;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

public readonly record struct CardDestination(Player Player, PileType PileType, CardPilePosition Position);

public interface ICardDestinationListener
{
    CardDestination ModifyCardDestination(
        CardModel card, bool isAutoPlay, ResourceInfo resources, CardDestination destination) => destination;

    Task AfterCardDestinationModified(CardModel card, CardDestination destination) => Task.CompletedTask;
}

// ==================== Beta branch (CardLocation exists) ====================

[HarmonyPatch]
internal static class ModifyCardDestinationPatch_Beta
{
    private static bool Prepare() => GameVersion.HasCardLocation;

    private static MethodBase TargetMethod() =>
        AccessTools.Method(typeof(Hook), nameof(Hook.ModifyCardPlayResultLocation));

    private static void Postfix(
        ICombatState combatState, CardModel card, bool isAutoPlay, ResourceInfo resources,
        ref CardLocation __result, ref IEnumerable<AbstractModel> modifiers)
    {
        var result = HookUtils.Modify<ICardDestinationListener, CardDestination>(
            combatState,
            new CardDestination(__result.player, __result.pileType, __result.position),
            (m, d) => m.ModifyCardDestination(card, isAutoPlay, resources, d),
            out var extra);

        __result = new CardLocation(result.Player, result.PileType, result.Position);

        var added = extra.OfType<AbstractModel>().ToList();
        if (added.Count > 0)
            modifiers = [.. modifiers, .. added];
    }
}

[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterModifyingCardPlayResultLocation))]
internal static class AfterCardDestinationModifiedPatch_Beta
{
    private static bool Prepare() => GameVersion.HasCardLocation;

    private static bool Prefix(AbstractModel __instance, CardModel card, CardLocation cardLocation, ref Task __result)
    {
        if (__instance is not ICardDestinationListener listener) return true;

        __result = listener.AfterCardDestinationModified(
            card, new CardDestination(cardLocation.player, cardLocation.pileType, cardLocation.position));
        return false;
    }
}

// ==================== Old branch (no CardLocation, PileType/CardPilePosition tuple) ====================

[HarmonyPatch]
internal static class ModifyCardDestinationPatch_Old
{
    private static bool Prepare() => !GameVersion.HasCardLocation;

    private static MethodBase TargetMethod() =>
        AccessTools.Method(typeof(Hook), "ModifyCardPlayResultPileTypeAndPosition");

    private static void Postfix(
        ICombatState combatState, CardModel card, bool isAutoPlay, ResourceInfo resources,
        ref (PileType, CardPilePosition) __result, ref IEnumerable<AbstractModel> modifiers)
    {
        var result = HookUtils.Modify<ICardDestinationListener, CardDestination>(
            combatState,
            new CardDestination(card.Owner, __result.Item1, __result.Item2),
            (m, d) => m.ModifyCardDestination(card, isAutoPlay, resources, d),
            out var extra);

        __result = (result.PileType, result.Position);

        var added = extra.OfType<AbstractModel>().ToList();
        if (added.Count > 0)
            modifiers = [.. modifiers, .. added];
    }
}

[HarmonyPatch(typeof(AbstractModel), "AfterModifyingCardPlayResultPileOrPosition")]
internal static class AfterCardDestinationModifiedPatch_Old
{
    private static bool Prepare() => !GameVersion.HasCardLocation;

    private static bool Prefix(
        AbstractModel __instance, CardModel card, PileType pileType, CardPilePosition position, ref Task __result)
    {
        if (__instance is not ICardDestinationListener listener) return true;

        __result = listener.AfterCardDestinationModified(card, new CardDestination(card.Owner, pileType, position));
        return false;
    }
}
