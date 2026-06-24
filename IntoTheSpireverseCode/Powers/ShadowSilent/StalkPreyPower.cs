using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowSilent;

public sealed class StalkPreyPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _skillCount;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player && cardPlay.Card.Type == CardType.Skill)
        {
            _skillCount++;
            if (_skillCount % 3 == 0)
            {
                Flash();
                await PowerCmd.Apply<InstinctPower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
            }
        }
    }
}
