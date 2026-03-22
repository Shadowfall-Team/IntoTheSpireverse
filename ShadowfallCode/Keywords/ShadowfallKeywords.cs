using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Patches;

namespace Shadowfall.ShadowfallCode.Keywords;

public static class ShadowfallKeywords
{
    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Devious;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Cunning;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Bleed;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Instinct;

    [CustomEnum] [KeywordProperties(AutoKeywordPosition.None)]
    public static CardKeyword Linger;

    public static bool IsCunningTriggered(CardModel card) =>
        HandPositionTrackingPatch.WasLeftmostInHand.TryGetValue(card, out bool val) && val;

    public static bool IsCunningActive(CardModel card) =>
        card.Pile?.Type == PileType.Hand && card.Pile.Cards.Count > 0 && card.Pile.Cards[0] == card;

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
        await CardCmd.Discard(context, card);

        for (int i = 0; i < repeats; i++)
            await effect();
    }

    public static async Task ExecuteLinger(PlayerChoiceContext context, Player player, AbstractModel source, Func<Task> effect)
    {
        
    }
}
