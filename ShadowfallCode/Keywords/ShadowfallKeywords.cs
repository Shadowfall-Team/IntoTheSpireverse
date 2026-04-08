using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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

    public static bool IsCunningTriggered(CardModel card) =>
        HandPositionTrackingPatch.WasLeftmostInHand.TryGetValue(card, out bool val) && val;

    public static bool IsCunningActive(CardModel card) =>
        card.Pile?.Type == PileType.Hand && card.Pile.Cards.Count > 0 && card.Pile.Cards[0] == card;

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
}
