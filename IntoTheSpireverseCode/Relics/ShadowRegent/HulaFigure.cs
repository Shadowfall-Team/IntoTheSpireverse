using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;

// Hula Figure (replacing Vitruvian Minion): Warp now grants 3E, but no Strength.

public class HulaFigure : ShadowRegentRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(3)
    ];
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Warp>()
    ];
}