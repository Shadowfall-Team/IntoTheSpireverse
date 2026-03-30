using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

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
        if (side != CombatSide.Enemy)
            return;

        var volleyDamage = DynamicVars.Damage.BaseValue +
                           Owner.GetPowerAmount<VolleyDamageThisTurn>();

        for (var i = 0; i < Amount; i++)
        {
            var validTargets = CombatState.Enemies.Where(e => e.IsAlive);
            var target = CombatState.RunState.Rng.CombatTargets.NextItem(validTargets);
            if (target != null)
            {
                await CreatureCmd.Damage(choiceContext, target, volleyDamage,
                    ValueProp.Unpowered, Owner);
            }
        }

        await PowerCmd.Remove<VolleyDamageThisTurn>(Owner);
        await PowerCmd.Remove(this);
    }
}