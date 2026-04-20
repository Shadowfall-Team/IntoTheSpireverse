using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Cards.ShadowSilent;

public sealed class Ambush() : ShadowSilentCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("Increase", 2m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    private decimal ExtraDamageFromPlays { get; set; }

    public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner == Owner && card != this && ShadowfallKeywords.WasAdjacentWhenRemoved(card, this))
        {
			DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
			ExtraDamageFromPlays += DynamicVars["Increase"].BaseValue;
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
	{
		DynamicVars["Increase"].UpgradeValueBy(1m);
	}
}
