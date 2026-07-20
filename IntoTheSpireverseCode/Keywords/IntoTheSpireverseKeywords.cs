using BaseLib.Extensions;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;

#if SILENT
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;
#endif

namespace IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

public static class IntoTheSpireverseKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Devious;

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
        return i >= 0 && j >= 0 && System.Math.Abs(i - j) == 1;
    }

    public static async Task ExecuteDevious(PlayerChoiceContext context, Player player, AbstractModel source, Func<Task> effect)
    {
        CardModel? card = (await CardSelectCmd.FromHandForDiscard(
            context,
            player,
            new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1),
            null,
            source)).FirstOrDefault();

        if (card == null)
            return;

        int repeats = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (card.EnergyCost.CostsX && player.PlayerCombatState != null)
            repeats = player.PlayerCombatState.Energy;
#if SILENT
        repeats += card is Weight ? player.Creature.GetPowerAmount<TipTheScalesPower>() : 0;
#endif
        await CardCmd.Discard(context, card);
        
        foreach (var model in card.Owner.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IModifyDeviousListener deviousListener)
                repeats = deviousListener.ModifyDeviousValue(card, repeats);
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
        void OnMuddled();
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

    public static void ApplyMuddle(CardModel card)
    {
        if (!CanMuddle(card))
            return;

        int currentCost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        int newCost;
        int maxCostReduce = 0;
#if SILENT
        if (card.Owner.Creature.HasPower<SharpWitPower>())
        {
            maxCostReduce = card.Owner.Creature.GetPowerAmount<SharpWitPower>();
        }
#endif

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
        
        foreach (var model in card.Owner.Creature.CombatState!.IterateHookListeners().ToList())
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
            listener.OnMuddled();
        
        foreach (var model in card.Owner.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is ICardMuddledListener powerListener)
                powerListener.AfterCardMuddled(card.Owner.Creature.CombatState, card);
        }
    }

    public static void ApplyMuddleAll(IEnumerable<CardModel> cards)
    {
        foreach (var card in cards)
            ApplyMuddle(card);
    }

    public static void ApplyMuddleHand(Player player)
    {
        ApplyMuddleAll(
            PileType.Hand.GetPile(player).Cards
                .Where(CanMuddle)
        );
    }

    public static void ApplyMuddleRandom(Player player, int count, Rng rng)
    {
        var eligible = PileType.Hand.GetPile(player).Cards
            .Where(CanMuddle)
            .ToList();

        for (int i = 0; i < count && eligible.Count > 0; i++)
        {
            var card = rng.NextItem(eligible);
            ApplyMuddle(card);
            eligible.Remove(card);
        }
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

        foreach (var card in selected)
            ApplyMuddle(card);

        return selected;
    }
}
