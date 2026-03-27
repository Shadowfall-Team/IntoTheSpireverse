using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class StrikeTrue() : ShadowSilentCard(-1, CardType.Skill, CardRarity.Rare, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable, CardKeyword.Ethereal];

    public override decimal ModifyDamageMultiplicative(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (cardSource != null
            && cardSource.Owner == Owner
            && cardSource.Type == CardType.Attack
            && (ShadowfallKeywords.IsCurrentlyAdjacent(cardSource, this)
                || ShadowfallKeywords.WasAdjacentWhenRemoved(cardSource, this)))
        {
            return 2m;
        }

        return 1m;
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}
