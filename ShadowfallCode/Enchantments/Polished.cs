using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Enchantments;

public sealed class Polished : ShadowfallEnchantments
{
    public override bool CanEnchantCardType(CardType cardType) => cardType == CardType.Attack;

    public override bool HasExtraCardText => false;

    public override bool ShowAmount => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
    ];

    protected override void OnEnchant() => Card.AddKeyword(CardKeyword.Retain);

    public override decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)
    {
        return !props.IsPoweredAttack() ? 0m : DynamicVars.Damage.BaseValue;
    }
}