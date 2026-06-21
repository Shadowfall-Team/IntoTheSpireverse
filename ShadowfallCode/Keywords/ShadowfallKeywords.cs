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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;
using Shadowfall.ShadowfallCode.Patches;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Keywords;

public static class ShadowfallKeywords
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
    public static CardKeyword Muddle;
    
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Overflow;
    
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Startup;
    
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Pickup;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Cargo;
    
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
        repeats += card is Ward ? player.Creature.GetPowerAmount<TipTheScalesPower>() : 0;
        await CardCmd.Discard(context, card);

        for (int i = 0; i < repeats; i++)
            await effect();
    }

    public static bool IsGloryTriggered(CardModel card)
    {
        var gloryVar = card.DynamicVars.GetValueOrDefault(GloryVar.defaultName);
        return gloryVar != null && IsGloryTriggered(card, gloryVar.IntValue);
    }
    
    public static bool IsGloryTriggered(CardModel card, int threshold)
    {
        var playsThisTurn = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.HappenedThisTurn(card.CombatState) && e.CardPlay.Card.Owner == card.Owner);
        return playsThisTurn >= threshold;
    }
    
    public class GloryVar(decimal amount, string? name = null) : DynamicVar(name ?? defaultName, amount)
    {
        public const string defaultName = "Glory";

        public override void UpdateCardPreview(
            CardModel card,
            CardPreviewMode previewMode,
            Creature? target,
            bool runGlobalHooks)
        {
            if (IsGloryTriggered(card, IntValue))
            {
                PreviewValue = BaseValue;
                
                // Hacky solution to make Glory values glow green when triggered, since
                // Glory isn't related to enchantments at all.
                // Lowering EnchantedValue below PreviewValue forces a positive comparison
                // in ToHighlightedString, which renders the number green.
                
                EnchantedValue = BaseValue - 1;
            }
            else
            {
                ResetToBase();
            }
        }
    }
    
    public static IHoverTip GloryHoverTipDynamic(DynamicVar gloryVar)
    {
        var title = new LocString("static_hover_tips", "SHADOWFALL_GLORY_DYNAMIC.title");
        var description = new LocString("static_hover_tips", "SHADOWFALL_GLORY_DYNAMIC.description");
        title.Add(gloryVar);
        description.Add(gloryVar);
        return new HoverTip(title, description);
    }
    
    public static IHoverTip GloryHoverTipStatic()
    {
        var title = new LocString("static_hover_tips", "SHADOWFALL_GLORY_STATIC.title");
        var description = new LocString("static_hover_tips", "SHADOWFALL_GLORY_STATIC.description");
        return new HoverTip(title, description);
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

    public static void ApplyMuddle(CardModel card)
    {
        if (!CanMuddle(card))
            return;

        int currentCost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        int newCost;

        if (currentCost >= 0 && currentCost <= 3)
        {
            newCost = card.Owner.RunState.Rng.CombatEnergyCosts.NextInt(3);
            if (newCost >= currentCost)
                newCost++;
        }
        else
        {
            newCost = card.Owner.RunState.Rng.CombatEnergyCosts.NextInt(4);
        }

        card.EnergyCost.SetThisTurnOrUntilPlayed(newCost);
        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();

        if (card is IMuddleListener listener)
            listener.OnMuddled();
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
                new LocString("card_selection", "SHADOWFALL_MUDDLE_PROMPT"),
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
    
    public static bool IsOverflowActive(CardModel card)
    {
        var hand = card.Owner.PlayerCombatState?.Hand;
        if (hand == null)
            return false;

        // Exclude the card itself, matching Follow Through's approach
        return hand.Cards.Count(c => c != card) >= 5;
    }

}
