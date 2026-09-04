using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

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

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Scry;
    
    /// True when the card reached the Play pile from anywhere other than Hand.
    public static bool WasPlayedIndirectly(CardModel card) =>
        IndirectPlayTrackingPatch.LastPileLeft.TryGetValue(card, out var pile) && pile != PileType.Hand;

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

    public static async Task ExecuteDevious(PlayerChoiceContext context, Player player, AbstractModel source, int repeats, Func<Task> effect)
    {
        int maxDiscards = 1;
        foreach (var model in player.Creature.CombatState?.IterateHookListeners().ToList()!)
        {
            if (model is IDeviousDiscardListener deviousListener)
                maxDiscards = deviousListener.ModifyDeviousDiscard(maxDiscards);
        }
        
        var cards = (await CardSelectCmd.FromHandForDiscard(
            context,
            player,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1, Math.Max(maxDiscards,1)),
            null,
            source));
        
        foreach (CardModel card in cards)
        {
            if (card.Owner.Creature.CombatState == null) return;

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

        for (int i = 0; i < repeats; i++)
            await effect();
    }
    
    public static bool CanMuddle(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable)
               && !card.EnergyCost.CostsX;
    }
    
    public interface IMuddleListener
    {
        Task OnMuddled();
    }
    
    public interface ICardMuddledListener
    {
        Task AfterCardMuddled(ICombatState combatState, CardModel cardModel);
    }
    
    public interface IShouldPermanentMuddleListener
    {
        bool ShouldPermanentMuddle(CardModel card);
    }
    
    public interface IModifyDeviousListener
    {
        int ModifyDeviousValue(CardModel card, int originalValue);
    }
    
    public interface IDeviousDiscardListener
    {
        int ModifyDeviousDiscard(int originalAmount);
    }

    public static async Task<CardModel?> ApplyMuddle(CardModel card)
    {
        if (card.Owner.Creature.CombatState == null) return null;
        if (!CanMuddle(card))
            return null;

        int currentCost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        int newCost;
        int maxCostReduce = 0;
        if (card.Owner.Creature.HasPower<OathOfDevotionPower>())
        {
            maxCostReduce = card.Owner.Creature.GetPowerAmount<OathOfDevotionPower>();
        }

        if (currentCost >= 0 && currentCost <= 3)
        {
            newCost = card.Owner.RunState.Rng.CombatEnergyCosts.NextInt(Math.Max(1,3 - maxCostReduce));
            if (newCost >= currentCost && maxCostReduce < 3)
                newCost++;
        }
        else
        {
            newCost = card.Owner.RunState.Rng.CombatEnergyCosts.NextInt(Math.Max(1,4 - maxCostReduce));
        }

        bool permanentMuddle = false;
        
        foreach (var model in card.Owner.Creature.CombatState.IterateHookListeners().ToList())
        {
            if (model is IShouldPermanentMuddleListener muddleListener)
                permanentMuddle |= muddleListener.ShouldPermanentMuddle(card);
        }
        
        if (permanentMuddle)
            card.EnergyCost.SetThisCombat(newCost);
        else
            card.EnergyCost.SetThisTurnOrUntilPlayed(newCost);
        
        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();

        if (card is IMuddleListener listener)
            await listener.OnMuddled();

        foreach (var model in card.Owner.Creature.CombatState.IterateHookListeners().ToList())
        {
            if (model is ICardMuddledListener powerListener)
                await powerListener.AfterCardMuddled(card.Owner.Creature.CombatState, card);
        }
        return card;
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleAll(IEnumerable<CardModel> cards)
    {
        List<CardModel> _cards = [];
        foreach (var card in cards)
        {
            var _card = await ApplyMuddle(card);
            if (_card is not null)
                _cards.Add(_card);
        }
        return _cards.Count == 0 ? [] : (IEnumerable<CardModel>)_cards;
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleHand(Player player)
    {
        return await ApplyMuddleAll(
            PileType.Hand.GetPile(player).Cards
                .Where(CanMuddle)
        );
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleRandom(Player player, int count, Rng rng)
    {
        var eligible = PileType.Hand.GetPile(player).Cards
            .Where(CanMuddle)
            .ToList();

        List<CardModel> _cards = [];
        for (int i = 0; i < count && eligible.Count > 0; i++)
        {
            var card = rng.NextItem(eligible);
            if (card != null)
            {
                await ApplyMuddle(card);
                _cards.Add(card);
                eligible.Remove(card);
            }
        }
        return _cards.Count == 0 ? [] : (IEnumerable<CardModel>) _cards;
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleFromHandSelection(
        PlayerChoiceContext choiceContext,
        Player player,
        AbstractModel source,
        int count = 1)
    {
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            new CardSelectorPrefs(
                new LocString("card_selection", "INTOTHESPIREVERSE-MUDDLE_PROMPT"),
                count,
                count
            ),
            CanMuddle,
            source
        );

        foreach (var card in selected ?? [])
            await ApplyMuddle(card);

        return selected ?? [];
    }
}
