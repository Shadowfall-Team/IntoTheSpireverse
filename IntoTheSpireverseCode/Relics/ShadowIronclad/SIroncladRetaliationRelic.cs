using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowIronclad;

public class ToyCactus : ShadowIroncladRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RetaliationPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new("DamageReduction", 2m),
    ];
}