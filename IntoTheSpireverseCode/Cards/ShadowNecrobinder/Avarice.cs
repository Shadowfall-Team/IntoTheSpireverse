/*
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowNecrobinder;

public sealed class Avarice() : ShadowNecrobinderCard(-1, CardType.Curse, CardRarity.Uncommon, TargetType.None)
{
    private const string _goldKey = "Gold";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(_goldKey, 100m),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable,
        IntoTheSpireverseKeywords.Pickup
    ];

    public override int MaxUpgradeLevel => 0;

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPile, AbstractModel? source)
    {
        if (card == this && card.Pile?.Type == PileType.Deck)
        {
            await PlayerCmd.GainGold(DynamicVars[_goldKey].IntValue, Owner);
        }
    }
}
*/
