using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class CardPlayToCargoPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature == Owner)
        {
            await CardPileCmd.Add(cardPlay.Card, CargoCardPile.CargoPileType);
            await PowerCmd.Remove(this);
        }
        
    }
}