using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

[Pool(typeof(StatusCardPool))]
public sealed class Weight() : CustomCardModel(1, CardType.Status, CardRarity.Token, TargetType.None)
{
    public override int MaxUpgradeLevel => 0;

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];

    public override Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player || Pile?.Type != PileType.Hand)
            return Task.CompletedTask;

        int currentBaseCost = EnergyCost.GetWithModifiers(CostModifiers.None);
        if (currentBaseCost < 3)
            EnergyCost.SetCustomBaseCost(currentBaseCost + 1);

        return Task.CompletedTask;
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card == this)
            await CardCmd.Exhaust(choiceContext, this);
    }
}
