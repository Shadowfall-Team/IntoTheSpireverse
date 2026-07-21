using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

public class PaperSnecko : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    
    private const string multiplierKey = "Multiplier";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(multiplierKey, 25M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
    ];
    
    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource, 
        CardPlay? cardPlay)
    {
        if (!props.IsPoweredAttack() || cardSource == null)
            return 1m;
        if (dealer != Owner.Creature && !Owner.Creature.Pets.Contains(dealer))
            return 1m;
        if (target == null || !target.HasPower<PoisonPower>())
            return 1m;

        return 1m + DynamicVars[multiplierKey].BaseValue / 100m;
    }
}