using BaseLib.Extensions;
﻿using HarmonyLib;
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
/// Mud reads "When Transformed, gain 1 Slate". The engine's transform notification
/// (<see cref="CardModel.AfterTransformedFrom"/>) is synchronous, but applying a power is not,
/// so Muds consumed during a transform are queued here and paid out by an async continuation
/// chained onto <see cref="CardCmd.Transform"/>'s task. Queue-then-flush also handles batch
/// transforms (Avalanche, Caldera) correctly.
/// </summary>
public static class MudTransformPatches
{
    private static readonly List<Mud> PendingMud = [];

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterTransformedFrom))]
    public static class MudTransformedFromPatch
    {
        public static void Postfix(CardModel __instance)
        {
            if (__instance is Mud mud && CombatManager.Instance.IsInProgress)
                PendingMud.Add(mud);
        }
    }

    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.Transform),
        [typeof(IEnumerable<CardTransformation>), typeof(Rng), typeof(CardPreviewStyle)])]
    public static class MudTransformPayoutPatch
    {
        public static void Postfix(ref Task<IEnumerable<CardPileAddResult>> __result)
        {
            __result = PayOutPendingMud(__result);
        }
    }

    private static async Task<IEnumerable<CardPileAddResult>> PayOutPendingMud(
        Task<IEnumerable<CardPileAddResult>> inner)
    {
        var results = await inner;

        if (PendingMud.Count == 0) return results;

        var muds = PendingMud.ToList();
        PendingMud.Clear();

        foreach (var mud in muds)
        {
            var creature = mud.Owner?.Creature;
            if (creature == null || creature.CombatState == null) continue;

            await PowerCmd.Apply<SlatePower>(
                new ThrowingPlayerChoiceContext(),
                creature, mud.DynamicVars.Power<SlatePower>().BaseValue,
                creature, null);
        }

        return results;
    }
}
