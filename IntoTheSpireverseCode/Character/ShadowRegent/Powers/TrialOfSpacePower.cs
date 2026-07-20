using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

public class TrialOfSpacePower : ShadowPowerModel, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public int Cards
    {
        get;
        set
        {
            field = value;
            InvokeDisplayAmountChanged();
        }
    }

    public string GetSecondAmount() => Cards.ToString();

    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (Owner.Player == null) return;
        if (side == CombatSide.Enemy)
            return;

        Flash();

        if (Owner.Player.PlayerCombatState?.Energy >= 3)
        {
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
            await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner, Cards, Owner, null);

            await PowerCmd.Remove(this);
        }
    }
}