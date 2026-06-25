using MegaCrit.Sts2.Core.Entities.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless.Rocks;

public abstract class RockCardBase(int cost, CardType type, CardRarity rarity, TargetType targetType)
    : IntoTheSpireverseCard(cost, type, rarity, targetType, "ironclad")
{
    private decimal ExtraDamageFromRockPlays
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    protected override HashSet<CardTag> CanonicalTags => [IntoTheSpireverseCardTags.Rock];

    protected void BuffFromRockPlay(decimal extraDamage)
    {
        DynamicVars.Damage.BaseValue += extraDamage;
        ExtraDamageFromRockPlays += extraDamage;
    }

    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamageFromRockPlays;
    }
}