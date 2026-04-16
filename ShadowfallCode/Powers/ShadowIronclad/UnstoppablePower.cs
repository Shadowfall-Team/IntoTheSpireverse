using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class UnstoppablePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack || cardPlay.Card.Owner.Creature != Owner)
            return;

        Flash();
        await CreatureCmd.GainBlock(Owner, (decimal)Amount, ValueProp.Unpowered, (CardPlay?)null);
    }
}