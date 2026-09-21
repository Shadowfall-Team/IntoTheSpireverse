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
    
    private bool _potionRockResolving;

    public override Task BeforePotionUsed(PotionModel potion, Creature? target)
    {
        _potionRockResolving = potion is PotionShapedRock && potion.Owner == Owner.Player;
        return Task.CompletedTask;
    }

    public override Task AfterPotionUsed(PotionModel potion, Creature? target)
    {
        _potionRockResolving = false;
        return Task.CompletedTask;
    }

    public decimal ModifyDamageAdditiveCompability(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer != Owner) return 0m;

        if (cardSource == null) return _potionRockResolving ? Amount : 0m;

        if (!props.IsPoweredAttack()) return 0m;
        if (!cardSource.Tags.Contains(IntoTheSpireverseCardTags.Rock)) return 0m;
        return Amount;
    }
}
