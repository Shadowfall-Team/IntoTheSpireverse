using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class TemperPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card.Type != CardType.Skill) return;

        // The history already includes this play, so the first Skill of the turn counts as 1.
        var skillsThisTurn = CombatManager.Instance.History.Entries.OfType<CardPlayFinishedEntry>()
            .Count(e => e.CardPlay.Card.Owner == Owner.Player
                        && e.CardPlay.Card.Type == CardType.Skill
                        && e.HappenedThisTurn(CombatState));
        if (skillsThisTurn > 1) return;

        Flash();
        await PowerCmd.Apply<TemperTemporaryStrengthPower>(choiceContext, Owner, (decimal)Amount, Owner, null);
    }
}
