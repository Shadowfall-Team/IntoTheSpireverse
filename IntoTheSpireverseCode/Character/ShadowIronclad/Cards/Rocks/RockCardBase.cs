using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Enchantments;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards.Rocks;

public abstract class RockCardBase(int cost, CardType type, CardRarity rarity, TargetType targetType)
    : IntoTheSpireverseCard(cost, type, rarity, targetType, "ironclad"), IRockCard
{
    private decimal _extraDamageFromRockPlays;

    public override void AfterThisWasTransformedFrom(CardModel card)
    {
        if (card.Type == CardType.Status)
        {
            var mudIdol = Owner.Relics.OfType<MudIdol>().FirstOrDefault();
            if (mudIdol is null) { return; }

            mudIdol.Flash();
            CardCmd.Upgrade(this);
            CardCmd.Enchant<Polished>(this, 1);
        }
    }

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