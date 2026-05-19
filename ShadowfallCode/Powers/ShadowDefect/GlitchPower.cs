using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowDefect;

public class GlitchPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    //TODO clarify if all players can proc it and how it interacts with upgraded version
    // public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    public const int BaseDamage = 8;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new GlitchDamageVar(this)];

    private CardModel? _applyingCard;

    public override Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    {
        _applyingCard = cardSource;
        return Task.CompletedTask;
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == _applyingCard)
        {
            _applyingCard = null;
            return;
        }
        _applyingCard = null;

        Flash();
        await CreatureCmd.Damage(choiceContext, Owner, BaseDamage, ValueProp.Unblockable | ValueProp.Unpowered, null, null);

        if (Owner.IsAlive)
            await PowerCmd.Decrement(this);
    }

    private class GlitchDamageVar(GlitchPower power) : DynamicVar("Damage", BaseDamage)
    {
        public override string ToString() => BaseDamage.ToString();

        protected override decimal GetBaseValueForIConvertible() => BaseDamage;
    }
}
