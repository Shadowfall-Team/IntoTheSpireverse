using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class WeightedStrike() : ShadowSilentCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(2m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(
            (card, _) =>
            {
                var hand = PileType.Hand.GetPile(card.Owner).Cards;
                return hand.Count(c => c != card);
            }),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.CalculatedDamage).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}
