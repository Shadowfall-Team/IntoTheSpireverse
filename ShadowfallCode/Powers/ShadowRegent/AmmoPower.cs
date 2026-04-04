using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext,
        CombatSide side)
    {
        if (side == CombatSide.Enemy)
            return;

        var volleyDamage = DynamicVars.Damage.BaseValue +
                           Owner.GetPowerAmount<VolleyDamageThisTurnPower>();

        for (var i = 0; i < Amount; i++)
        {
            var validTargets = CombatState.Enemies.Where(e => e.IsAlive).ToList();
            var preferredTargets = validTargets.Where(t => t.HasPower<TargettedThisTurnPower>()).ToList();

            var target = CombatState.RunState.Rng.CombatTargets.NextItem(
                preferredTargets.Count != 0 ? preferredTargets : validTargets);
            if (target != null)
            {
                //TODO: maybe play an animation here?
                // VfxCmd.PlayOnCreatureCenter(attackCommand.Attacker, attackCommand._attackerVfx);

                await CreatureCmd.Damage(choiceContext, target, volleyDamage,
                    ValueProp.Unpowered, Owner);
            }
        }

        await PowerCmd.Remove<VolleyDamageThisTurnPower>(Owner);
        foreach (var target in
                 CombatState.Enemies.Where(e => e.HasPower<TargettedThisTurnPower>()))
        {
            await PowerCmd.Remove<TargettedThisTurnPower>(target);
        }

        await PowerCmd.Remove(this);
    }
}