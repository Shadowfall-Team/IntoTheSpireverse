using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class UnrelentingFormPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (!(player.PlayerCombatState is { Phase: PlayerTurnPhase.Play })
            || player != Owner.Player
            || cardPlay.Card.Type != CardType.Attack
            || CombatManager
                .Instance.History.Entries.OfType<CardPlayFinishedEntry>()
                .Count(e =>
                    e.CardPlay.Card.Type == CardType.Attack && e.HappenedThisTurn(CombatState)
                ) > Amount
        ) return;

        Flash();
        await PlayerCmd.GainEnergy(1, Owner.Player);
        await CardPileCmd.Draw(choiceContext, 1, Owner.Player);
    }
}
