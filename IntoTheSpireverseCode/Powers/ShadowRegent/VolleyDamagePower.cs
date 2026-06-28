using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class VolleyDamagePower: ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? card)
    {
        if (Owner != dealer || !props.IsPoweredAttack() || card is null || card is not AmmoVolley)
            return 0m;
        return Amount;
    }
}
