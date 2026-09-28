using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Patches;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class GabbroPower : ShadowPowerModel, IModifyDamageAdditive
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    /// <summary>
    /// The damage Gabbro adds to a Potion-Shaped Rock, for PotionShapedRockPatches.
    /// </summary>
    public static decimal PotionRockBonus(PotionModel potion)
    {
        if (potion is not PotionShapedRock || !potion.IsMutable) return 0m;
        return potion.Owner?.Creature.Powers.OfType<GabbroPower>().FirstOrDefault()?.Amount ?? 0m;
    }

    public decimal ModifyDamageAdditiveCompability(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner || cardSource == null) return 0m;
        if (!props.IsPoweredAttack()) return 0m;
        if (!cardSource.Tags.Contains(IntoTheSpireverseCardTags.Rock)) return 0m;
        return Amount;
    }
}
