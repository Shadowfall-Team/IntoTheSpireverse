using System.Reflection;
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

// Does gaining slate have to be async? or is this done so that attacks which transform don't trigger themselves

/// <summary>
/// <see cref="CardModel.AfterTransformedFrom"/> is synchronous and payouts are async, so cards
/// that pay out when transformed are settled once their <see cref="CardCmd.Transform"/> task ends.
/// </summary>
public static class TransformPayoutPatches
{
    static readonly MethodInfo? v111AfterTransformedFrom;
    static readonly MethodInfo? v113AfterThisWasTransformedInto;
    static TransformPayoutPatches()
    {
        v111AfterTransformedFrom = AccessTools.Method(typeof(CardModel), "AfterTransformedFrom");
        v113AfterThisWasTransformedInto = AccessTools.Method(typeof(CardModel), nameof(CardModel.AfterThisWasTransformedInto), [typeof(CardModel)]);
        if (v111AfterTransformedFrom is null && v113AfterThisWasTransformedInto is null)
        {
            throw new MethodAccessException("Neither CardModel.AfterTransformedFrom or a known CardModel.AfterThisWasTransformedInto signature found!");
        }
    }

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

    /// <summary>
    /// A Transform with no actual replacement card, specifically a hook for Tectonic's Alkalize.
    /// </summary>
    public static async Task TransformAway(PlayerChoiceContext choiceContext, CardModel card)
    {
        await CardPileCmd.RemoveFromCombat(card);
        if (v111AfterTransformedFrom is not null)
        {
            v111AfterTransformedFrom.Invoke(card, []);
        }
        else if (v113AfterThisWasTransformedInto is not null)
        {
            // TODO: Keep an eye out for if this breaks anything
            v113AfterThisWasTransformedInto.Invoke(card, [card]);// this isn't accurate since its "transformed" into a potion, not a card. 
        }
        if (card is ITransformPayout payout)
            await payout.OnTransformedAway(choiceContext);
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
