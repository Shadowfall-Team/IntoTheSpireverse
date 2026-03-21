using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public class BleedPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public const int BaseDamage = 6;

    private int ComputeDamage() => BaseDamage + (Owner?.CombatState != null
        ? Owner.CombatState.GetOpponentsOf(Owner).Sum(p => p.GetPowerAmount<InstinctPower>())
        : 0);

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BleedDamageVar(this)];

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, ComputeDamage(), ValueProp.Unblockable | ValueProp.Unpowered, null, null);

        if (Owner.IsAlive)
            await PowerCmd.Decrement(this);
    }

    private class BleedDamageVar(BleedPower power) : DynamicVar("Damage", BaseDamage)
    {
        public override string ToString() => power.ComputeDamage().ToString();

        protected override decimal GetBaseValueForIConvertible() => power.ComputeDamage();
    }
}
