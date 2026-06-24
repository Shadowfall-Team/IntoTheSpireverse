using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowNecrobinder;

public class BooBuddiesPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        if (card.Owner.Creature != Owner || card is not SoulStrike)
            return playCount;

        bool alreadyPlayedThisTurn = CombatManager.Instance.History.CardPlaysStarted
            .Any(e => e.Actor == Owner
                      && e.CardPlay.Card is SoulStrike
                      && e.CardPlay.IsFirstInSeries
                      && e.HappenedThisTurn(CombatState));

        return alreadyPlayedThisTurn ? playCount : playCount + Amount;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        return Task.CompletedTask;
    }
}