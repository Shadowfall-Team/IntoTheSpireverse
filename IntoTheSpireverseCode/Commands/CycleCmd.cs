using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Commands;

public static class CycleCmd
{
    public static async Task Cycle(PlayerChoiceContext choiceContext, Player player, int amount = 1)
    {
        if (amount <= 0) return;
        var hand = PileType.Hand.GetPile(player);

        List<CardModel> cardsToDiscard = []; 
        
        for (var i = 0; i < amount; i++)
        {
            if (i >= hand.Cards.Count) break;
            var leftmost = hand.Cards[i];
            cardsToDiscard.Add(leftmost);
        }

        await CardCmd.Discard(choiceContext, cardsToDiscard);
        await CardPileCmd.Draw(choiceContext, amount, player);
    }
}