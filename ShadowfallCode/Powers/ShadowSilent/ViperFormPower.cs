using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class ViperFormPower : CustomPowerModel
{
    // TODO when baselib gets twoamountpowers redo this to show remaining triggers left as well
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _triggersThisTurn;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner.Player
            && ShadowfallKeywords.WasRightmostWhenPlayed(cardPlay.Card)
            && _triggersThisTurn < Amount)
        {
            _triggersThisTurn++;
            Flash();
            await PlayerCmd.GainEnergy(1, Owner.Player);
            await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player _)
    {
        _triggersThisTurn = 0;
    }
}
