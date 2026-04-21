using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class RetaliationPower : CustomPowerModel
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

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (Owner.Side == side)
            return;
        if (Owner.HasPower<RevengePower>())
            return;
        await PowerCmd.Remove(this);
    }
}