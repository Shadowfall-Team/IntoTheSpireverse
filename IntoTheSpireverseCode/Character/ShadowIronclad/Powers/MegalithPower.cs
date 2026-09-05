using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class MegalithPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress) return;
        if (target != Owner || Owner.IsDead) return;
        if (result.UnblockedDamage <= 0 || Amount <= 0) return;

        int absorbed = Math.Min(result.UnblockedDamage, Amount);

        Flash();
        await CreatureCmd.Heal(Owner, absorbed, false);
        await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, -absorbed, Owner, null);
    }
}
