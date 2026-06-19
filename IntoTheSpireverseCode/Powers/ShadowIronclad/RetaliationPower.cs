using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;

public sealed class RetaliationPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult _,
        ValueProp props,
        Creature? dealer,
        CardModel? __)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack())
            return;
        await CreatureCmd.Damage(choiceContext, dealer, (decimal)Amount, ValueProp.Unpowered, Owner, (CardModel?)null);
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !props.IsPoweredAttack()) return 0m;

        var relic = Owner.Player?.Relics.OfType<ToyCactus>().FirstOrDefault();
        if (relic == null) return 0m;
        
        return -relic.DynamicVars["DamageReduction"].BaseValue;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (Owner.Side == side)
            return;
        if (Owner.HasPower<RevengePower>())
            return;
        await PowerCmd.Remove(this);
    }
}