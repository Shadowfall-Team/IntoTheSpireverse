using BaseLib.Extensions;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Statuses;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Random;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Some Tectonic cards pay out when they are Transformed away: Mud grants Slate, Pyroclast draws.
/// The engine's transform notification (<see cref="CardModel.AfterTransformedFrom"/>) is
/// synchronous and both payouts are async, so consumed cards are queued here and settled by an
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
            if (__instance is Mud or Pyroclast)
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
            var creature = card.Owner?.Creature;
            if (creature?.CombatState == null) continue;

            switch (card)
            {
                case Mud mud:
                    await PowerCmd.Apply<SlatePower>(
                        new ThrowingPlayerChoiceContext(),
                        creature, mud.DynamicVars.Power<SlatePower>().BaseValue,
                        creature, null);
                    break;

                case Pyroclast pyroclast:
                    await pyroclast.DrawPayout(new ThrowingPlayerChoiceContext());
                    break;
            }
        }

        return results;
    }
}
