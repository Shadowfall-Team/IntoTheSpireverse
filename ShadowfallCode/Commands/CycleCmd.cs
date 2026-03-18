using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Commands;

public static class CycleCmd
{
    public static async Task Cycle(PlayerChoiceContext choiceContext, Player player)
    {
        var hand = PileType.Hand.GetPile(player);

        var leftmost = hand.Cards.FirstOrDefault();
        if (leftmost == null)
            return;

        await CardCmd.Discard(choiceContext, leftmost);
        await CardPileCmd.Draw(choiceContext, 1M, player);
    }
}