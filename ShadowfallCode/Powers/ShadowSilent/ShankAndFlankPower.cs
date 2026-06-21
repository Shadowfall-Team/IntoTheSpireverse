using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class ShankAndFlankPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
    ];

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? card)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || card == null)
            return 0m;
        if (card.EnergyCost.GetWithModifiers(CostModifiers.All) != 0)
            return 0m;
        return (decimal)Amount;
    }

    public override decimal ModifyBlockAdditive(
        Creature target,
        decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (target != Owner || !props.IsPoweredCardOrMonsterMoveBlock() || cardSource == null)
            return 0m;
        if (cardSource.EnergyCost.GetWithModifiers(CostModifiers.All) != 0)
            return 0m;
        return (decimal)Amount;
    }

    public override Task AfterModifyingBlockAmount(
        decimal modifiedBlock,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        Flash();
        return Task.CompletedTask;
    }
}