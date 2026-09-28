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
/// <see cref="CardModel.AfterTransformedFrom"/> is synchronous and payouts are async, so consumed
/// cards are queued and paid out once <see cref="CardCmd.Transform"/>'s task settles.
/// </summary>
public static class TransformPayoutPatches
{
    private static readonly List<CardModel> Pending = [];

    public static void Clear() => Pending.Clear();

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
            await PayOut(card);

        return results;
    }

    private static async Task PayOut(CardModel card)
    {
        if (card.Owner?.Creature.CombatState == null) return;

        if (card is ITransformPayout payout)
            await payout.OnTransformedAway(new ThrowingPlayerChoiceContext());
    }
}
