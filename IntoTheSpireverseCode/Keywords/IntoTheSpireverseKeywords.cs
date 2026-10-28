using BaseLib.Patches.Content;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using IntoTheSpireverse.IntoTheSpireverseCode.Singletons;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

public static class IntoTheSpireverseKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Devious;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword DeviousX;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Cunning;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Instinct;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Linger;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Startup;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Muddle;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Pickup;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Cargo;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Arcane;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Modify;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Indirectly;

    /// <summary>
    /// True for any play other than the player playing the card from Hand. Replay repeats copy the
    /// original IsAutoPlay and never change piles, so IsFirstInSeries is the only way to catch them.
    /// The pile check covers a card reaching Play from another pile without going through AutoPlay.
    /// </summary>
    public static bool WasPlayedIndirectly(CardPlay cardPlay) =>
        cardPlay.IsAutoPlay
        || !cardPlay.IsFirstInSeries
        || (IndirectPlayTracker.TryGetLastPileLeft(cardPlay.Card, out var pile)
            && pile != PileType.Hand);

    public static bool WasRightmostWhenPlayed(CardModel card) =>
        HandPositionTrackingPatch.WasRightmostInHand.TryGetValue(card, out bool val) && val;

    public static bool IsRightmostActive(CardModel card) =>
        card.Pile?.Type == PileType.Hand && card.Pile.Cards.Count > 0 && card.Pile.Cards[^1] == card;

    public static bool WasAdjacentWhenRemoved(CardModel removedCard, CardModel neighbor) =>
        HandPositionTrackingPatch.AdjacentCards.TryGetValue(removedCard, out var list) && list.Contains(neighbor);

    public static bool IsCurrentlyAdjacent(CardModel a, CardModel b)
    {
        if (a.Pile?.Type != PileType.Hand || a.Pile != b.Pile)
            return false;
        var cards = a.Pile.Cards;
        int i = cards.IndexOf(a);
        int j = cards.IndexOf(b);
        return i >= 0 && j >= 0 && Math.Abs(i - j) == 1;
    }

    public static async Task<int> ModifyDeviousRepeatCount(PlayerChoiceContext context, Player player, AbstractModel source, int repeats)
    {
        int maxDiscards = 1;
        foreach (var model in player.Creature.CombatState?.IterateHookListeners().ToList()!)
        {
            if (model is IDeviousDiscardListener deviousListener)
                maxDiscards = deviousListener.ModifyDeviousDiscard(maxDiscards);
        }

        var cards = await CardSelectCmd.FromHandForDiscard(context, player,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1, Math.Max(maxDiscards,1)),
            null, source);

        foreach (CardModel card in cards)
        {
            if (card.Owner.Creature.CombatState == null) continue;

            repeats += Math.Max(0, card.EnergyCost.GetWithModifiers(CostModifiers.All));
            if (card.EnergyCost.CostsX && player.PlayerCombatState != null)
                repeats += Math.Max(0, player.PlayerCombatState.Energy);
            await CardCmd.Discard(context, card);

            foreach (var model in card.Owner.Creature.CombatState.IterateHookListeners().ToList())
            {
                if (model is IModifyDeviousListener deviousListener)
                    repeats = deviousListener.ModifyDeviousValue(card, repeats);
            }
        }
        return repeats;
    }

    public static async Task ExecuteDevious(PlayerChoiceContext context, Player player, AbstractModel source, int repeats, Func<Task> effect)
    {
        repeats = await ModifyDeviousRepeatCount(context, player, source, repeats);

        for (int i = 0; i < repeats; i++)
            await effect();
    }




    public interface IModifyDeviousListener
    {
        int ModifyDeviousValue(CardModel card, int originalValue);
    }

    public interface IDeviousDiscardListener
    {
        int ModifyDeviousDiscard(int originalAmount);
    }

}
