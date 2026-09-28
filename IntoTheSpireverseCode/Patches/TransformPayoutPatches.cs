using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

public interface ITransformPayout
{
    Task OnTransformedAway(PlayerChoiceContext choiceContext);
}

/// <summary>
/// <see cref="CardModel.AfterTransformedFrom"/> is synchronous and payouts are async, so cards
/// that pay out when transformed are settled once their <see cref="CardCmd.Transform"/> task ends.
/// </summary>
public static class TransformPayoutPatches
{
    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform),
        [typeof(IEnumerable<CardTransformation>), typeof(Rng), typeof(CardPreviewStyle)])]
    public static class TransformPayoutPatch
    {
        public static void Prefix(ref IEnumerable<CardTransformation> transformations,
            out List<ITransformPayout> __state)
        {
            __state = CombatManager.Instance.IsInProgress
                ? transformations.Select(t => t.Original).OfType<ITransformPayout>().ToList()
                : [];
        }

        public static void Postfix(ref Task<IEnumerable<CardPileAddResult>> __result,
            List<ITransformPayout> __state)
        {
            __result = SettlePayouts(__result, __state);
        }
    }

    private static async Task<IEnumerable<CardPileAddResult>> SettlePayouts(
        Task<IEnumerable<CardPileAddResult>> inner, List<ITransformPayout> payouts)
    {
        var results = await inner;

        foreach (var payout in payouts)
        {
            if (payout is not CardModel card || card.Owner?.Creature.CombatState == null)
                continue;

            await payout.OnTransformedAway(new ThrowingPlayerChoiceContext());
        }

        return results;
    }
}
