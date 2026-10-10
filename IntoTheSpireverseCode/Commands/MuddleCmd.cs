using IntoTheSpireverse.IntoTheSpireverseCode.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Commands;

public static class MuddleCmd
{
    public static bool CanMuddle(CardModel card)
    {
        return MuddleCore.CanMuddle(card);
    }

    public static Task<CardModel?> ApplyMuddle(PlayerChoiceContext ctx, CardModel card, bool lowerOnly = false)
    {
        return MuddleCore.Muddle(ctx, card, lowerOnly);
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleAll(
        PlayerChoiceContext choiceContext,
        IEnumerable<CardModel> cards)
    {
        List<CardModel> results = [];
        foreach (var card in cards)
        {
            if (await ApplyMuddle(choiceContext, card) is { } muddled)
                results.Add(muddled);
        }
        return results;
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleFromHandSelection(
        PlayerChoiceContext choiceContext,
        Player player,
        AbstractModel source,
        int count = 1)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "INTOTHESPIREVERSE-MUDDLE_PROMPT"),
            count, count
        );
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            CanMuddle,
            source
        );
        return await ApplyMuddleAll(choiceContext, selected);
    }


    public static async Task<IEnumerable<CardModel>> ApplyMuddleHand(PlayerChoiceContext choiceContext, Player player)
    {
        return await ApplyMuddleAll(choiceContext, PileType.Hand.GetPile(player).Cards.Where(CanMuddle));
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleRandom(
        PlayerChoiceContext choiceContext, Player player, int count, Rng rng)
    {
        var eligible = PileType.Hand.GetPile(player).Cards
            .Where(CanMuddle)
            .ToList();

        List<CardModel> muddled = [];
        while (muddled.Count < count && eligible.Count > 0)
        {
            var index = rng.NextInt(eligible.Count);
            var card = eligible[index];
            eligible.RemoveAt(index);
            if (await ApplyMuddle(choiceContext, card) is { } result)
                muddled.Add(result);
        }
        return muddled;
    }
}
