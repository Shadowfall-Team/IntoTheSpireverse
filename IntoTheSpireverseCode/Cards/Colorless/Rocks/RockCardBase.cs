using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Interfaces;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless.Rocks;

public abstract class RockCardBase(int cost, CardType type, CardRarity rarity, TargetType targetType)
    : IntoTheSpireverseCard(cost, type, rarity, targetType, "ironclad"), IRockCard
{
    private decimal _extraDamageFromRockPlays;

    protected decimal ExtraDamageFromRockPlays
    {
        get => _extraDamageFromRockPlays;
        private set
        {
            AssertMutable();
            _extraDamageFromRockPlays = value;
        }
    }

    protected override HashSet<CardTag> CanonicalTags => new() { IntoTheSpireverseCardTags.Rock };

    public virtual void BuffFromRockPlay(decimal extraDamage)
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