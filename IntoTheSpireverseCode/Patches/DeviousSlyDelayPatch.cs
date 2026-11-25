using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// <summary>
/// Defers only the native Sly autoplay caused by a card selected in a Devious discard prompt.
/// A candidate is registered immediately around that prompt's CardCmd.Discard call; the native
/// AutoPlayType.SlyDiscard callback is the confirmation that the discard actually produced Sly.
/// </summary>
public static class DeviousSlyDelayManager
{
    internal sealed class Session(PlayerChoiceContext context)
    {
        public PlayerChoiceContext Context { get; set; } = context;
        public List<CardModel> DeferredCards { get; } = [];
    }

    private static readonly ConditionalWeakTable<CardModel, Session> SessionsBySource = new();
    private static readonly Dictionary<CardModel, Session> PendingPromptDiscards = [];

    internal static Session? Begin(CardModel? source, PlayerChoiceContext context)
    {
        if (source is null) return null;
        var session = SessionsBySource.GetValue(source, _ => new Session(context));
        session.Context = context;
        return session;
    }

    internal static void BeginPromptDiscard(Session? session, CardModel card)
    {
        if (session is not null) PendingPromptDiscards[card] = session;
    }

    internal static void EndPromptDiscard(Session? session, CardModel card)
    {
        if (session is not null && PendingPromptDiscards.TryGetValue(card, out var registeredSession) && ReferenceEquals(registeredSession, session)) PendingPromptDiscards.Remove(card);
    }

    internal static bool TryCaptureNativeSly(CardModel card)
    {
        if (!PendingPromptDiscards.Remove(card, out var session)) return false;
        if (!session.DeferredCards.Contains(card)) session.DeferredCards.Add(card);
        return true;
    }

    private static bool TryTake(CardModel source, out Session? session)
    {
        if (!SessionsBySource.TryGetValue(source, out var typedSession))
        {
            session = null;
            return false;
        }

        SessionsBySource.Remove(source);
        session = typedSession;
        return true;
    }

    internal static async Task ReleaseAfterSourceEffects(Task sourceEffects, CardModel source)
    {
        Session? session = null;
        try
        {
            await sourceEffects;
            if (!TryTake(source, out session) || session is null) return;

            foreach (var card in session.DeferredCards)
            {
                if (card.HasBeenRemovedFromState || card.Pile?.Type is PileType.Exhaust or PileType.Play) continue;
                var context = IsOwnedByContext(card, session.Context) ? session.Context : new BlockingPlayerChoiceContext();
                await CardCmd.AutoPlay(context, card, null, AutoPlayType.SlyDiscard);
            }
        }
        finally
        {
            if (session is null) TryTake(source, out session);
            if (session is not null) foreach (var card in session.DeferredCards) if (PendingPromptDiscards.TryGetValue(card, out var registeredSession) && ReferenceEquals(registeredSession, session)) PendingPromptDiscards.Remove(card);
        }
    }

    private static bool IsOwnedByContext(CardModel card, PlayerChoiceContext context) => context.OwnerId is { } contextOwnerId && card.Owner is { } cardOwner && cardOwner.NetId == contextOwnerId;
}

/// <summary>Consumes a Devious prompt's native Sly autoplay until its source card has finished resolving.</summary>
[HarmonyPatch(typeof(CardCmd), nameof(CardCmd.AutoPlay), typeof(PlayerChoiceContext), typeof(CardModel), typeof(Creature), typeof(AutoPlayType), typeof(bool), typeof(bool))]// ReSharper disable once UnusedType.Global
internal static class DeviousSlyAutoPlayDelayPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]// ReSharper disable once UnusedMember.Local InconsistentNaming
    private static bool Prefix(CardModel card, AutoPlayType type, ref Task __result)
    {
        if (type is not AutoPlayType.SlyDiscard || !DeviousSlyDelayManager.TryCaptureNativeSly(card)) return true;

        __result = Task.CompletedTask;
        return false;
    }
}

/// <summary>Releases the captured Sly cards after the full source-card's OnPlayWrapper, including Devious repeats, completes.</summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]// ReSharper disable once UnusedType.Global
internal static class DeviousSlyReleaseAfterSourcePatch
{
    [HarmonyPostfix]// ReSharper disable UnusedMember.Local InconsistentNaming
    private static void Postfix(CardModel __instance, ref Task __result) => __result = DeviousSlyDelayManager.ReleaseAfterSourceEffects(__result, __instance);
}
