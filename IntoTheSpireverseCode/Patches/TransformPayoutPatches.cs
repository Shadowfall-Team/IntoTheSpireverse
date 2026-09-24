using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Implemented by cards that pay out when they are Transformed away.
///
/// The engine's transform notification (<see cref="CardModel.AfterTransformedFrom"/>) is synchronous
/// and these payouts are not, so <see cref="TransformPayoutPatches"/> queues implementors as they are
/// consumed and calls this once <see cref="CardCmd.Transform"/>'s task has settled.
/// </summary>
public interface ITransformPayout
{
    Task OnTransformedAway(PlayerChoiceContext choiceContext);
}

/// <summary>
/// Settles <see cref="ITransformPayout"/> for cards that were Transformed away.
///
/// The engine's transform notification (<see cref="CardModel.AfterTransformedFrom"/>) is
/// synchronous and the payouts are async, so consumed cards are queued here and settled by an
/// async continuation chained onto <see cref="CardCmd.Transform"/>'s task. Queue-then-flush also
/// makes batch transforms (Caldera, We Are Legion) resolve correctly.
/// </summary>
public static class TransformPayoutPatches
{
    private static readonly List<CardModel> Pending = [];

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterTransformedFrom))]
    public static class TransformedFromPatch
    {
        public static void Postfix(CardModel __instance)
        {
            if (!CombatManager.Instance.IsInProgress) return;
            if (__instance is ITransformPayout)
                Pending.Add(__instance);
        }
    }

    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform),
        [typeof(IEnumerable<CardTransformation>), typeof(Rng), typeof(CardPreviewStyle)])]
    public static class TransformPayoutPatch
    {
        public static void Postfix(ref Task<IEnumerable<CardPileAddResult>> __result)
        {
            __result = SettlePending(__result);
        }
    }

    private static async Task<IEnumerable<CardPileAddResult>> SettlePending(
        Task<IEnumerable<CardPileAddResult>> inner)
    {
        var results = await inner;

        if (Pending.Count == 0) return results;

        var settled = Pending.ToList();
        Pending.Clear();

        foreach (var card in settled)
        {
            // Owner rather than the card's own CombatState: that property is derived from the card's
            // current pile, and a Transformed card has already left its pile by the time we get here.
            if (card.Owner?.Creature.CombatState == null) continue;

            if (card is ITransformPayout payout)
                await payout.OnTransformedAway(new ThrowingPlayerChoiceContext());
        }

        return results;
    }
}
