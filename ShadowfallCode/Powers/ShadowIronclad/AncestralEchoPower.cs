using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class AncestralEchoPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.IsAutoPlay)
            return;

        Flash();

        var clone = cardPlay.Card.CreateClone();
        CardCmd.ApplyKeyword(clone, CardKeyword.Ethereal);
        await CardPileCmd.AddGeneratedCardToCombat(clone, PileType.Hand, true);

        await PowerCmd.Decrement(this);
    }
}