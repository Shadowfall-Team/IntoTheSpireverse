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
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;
using Shadowfall.ShadowfallCode.Patches;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Keywords;

public static class ShadowfallKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
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
        repeats += card is Weight ? player.Creature.GetPowerAmount<TipTheScalesPower>() : 0;
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
}
