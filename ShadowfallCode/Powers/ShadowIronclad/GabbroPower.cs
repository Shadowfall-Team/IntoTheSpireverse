using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.CardTags;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class GabbroPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (dealer != Owner || cardSource == null || !props.IsPoweredAttack()) return 0m;
        if (!cardSource.Tags.Contains(ShadowfallCardTags.Rock)) return 0m;
        return (decimal)Amount;
    }
}