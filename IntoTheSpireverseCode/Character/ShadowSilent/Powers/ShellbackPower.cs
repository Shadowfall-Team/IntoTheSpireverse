
using BaseLib.Abstracts;
using BaseLib.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards.Colorless;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class ShellbackPower : ShadowPowerModel, IHasSecondAmount
{
    private const string EnergyKey = "EnergyGain";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.ForEnergy(this),
        HoverTipFactory.FromCard<Scale>()
    ];  
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(EnergyKey, 0),
    ];
    
    public string GetSecondAmount() =>
        DynamicVars[EnergyKey].IntValue.ToString();
    
    public void AddCost(int value)
    {
        AssertMutable();
        DynamicVars[EnergyKey].BaseValue += value;
        this.InvokeSecondAmountChanged();
    }

    public override bool TryModifyEnergyCostInCombat(
        CardModel card,
        Decimal originalCost,
        out Decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card.Owner.Creature != Owner || !card.Tags.Contains(IntoTheSpireverseCardTags.Scale))
            return false;
        modifiedCost = originalCost + DynamicVars[EnergyKey].BaseValue;
        return true;
    }
    
    public override Decimal ModifyBlockAdditive(
        Creature target,
        Decimal block,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return Owner != target || !props.IsPoweredCardOrMonsterMoveBlock() || cardSource != null && !cardSource.Tags.Contains(IntoTheSpireverseCardTags.Scale) ? 0M : Amount;
    }

    public override Task AfterModifyingBlockAmount(
        Decimal modifiedBlock,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        Flash();
        return Task.CompletedTask;
    }
}