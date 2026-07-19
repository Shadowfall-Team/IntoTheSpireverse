using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.Powers;

public class GrimoireOfSoulsPower : IntoTheSpireversePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card is not SoulStrike || Owner.Player == null) return;
        Flash();
        await CardPileCmd.Draw(context, Amount, Owner.Player);
    }
}
