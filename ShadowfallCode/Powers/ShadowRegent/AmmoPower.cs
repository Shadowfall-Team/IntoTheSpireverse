using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Powers.ShadowRegent;

public class AmmoPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8, ValueProp.Move),
    ];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Enemy)
            return;


        for (var i = 0; i < Amount; i++)
        {
            //TODO: move this to a calculated var?
            var volleyDamage = DynamicVars.Damage.BaseValue +
                               Owner.GetPowerAmount<VolleyDamageThisTurnPower>() +
                               Owner.GetPowerAmount<VolleyDamagePower>();

            if (Owner.HasPower<StrengthVolleyPower>())
            {
                volleyDamage += Owner.GetPowerAmount<StrengthPower>();
            }
            
            var target = SelectTarget();

            if (target == null) return;
            
            //TODO: maybe play an animation here?
            // VfxCmd.PlayOnCreatureCenter(attackCommand.Attacker, attackCommand._attackerVfx);

            await CreatureCmd.Damage(choiceContext, target, volleyDamage,
                ValueProp.Unpowered, Owner);

            if (Owner.HasPower<CascadePower>())
            {
                await PowerCmd.Apply<VolleyDamagePower>(new ThrowingPlayerChoiceContext(), Owner, 1, Owner, null);
            }

            if (Owner.HasPower<SiegePower>())
            {
                await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, 1, Owner, null);
            }

            if (Owner.HasPower<DefensiveCannonadePower>())
            {
                await CreatureCmd.GainBlock(Owner, Owner.GetPowerAmount<DefensiveCannonadePower>(), ValueProp.Unpowered, null);
            }
        }

        await Cleanup();
    }

    private Creature? SelectTarget()
    {
        var validTargets = CombatState.Enemies.Where(e => e.IsAlive).ToList();
        var preferredTargets = validTargets
            .Where(t => t.HasPower<TargetedThisTurnPower>()).ToList();

        var target = CombatState.RunState.Rng.CombatTargets.NextItem(
            preferredTargets.Count != 0 ? preferredTargets : validTargets);
        return target;
    }

    private async Task Cleanup()
    {
        await PowerCmd.Remove<VolleyDamageThisTurnPower>(Owner);
        await PowerCmd.Remove<DefensiveCannonadePower>(Owner);
        foreach (var target in
                 CombatState.Enemies.Where(e => e.HasPower<TargetedThisTurnPower>()))
        {
            await PowerCmd.Remove<TargetedThisTurnPower>(target);
        }

        await PowerCmd.Remove(this);
    }
}