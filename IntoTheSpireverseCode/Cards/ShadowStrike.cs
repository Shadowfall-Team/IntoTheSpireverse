using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards;

public sealed class ShadowStrike : ShadowDefectCard
{
	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new DamageVar(5m, ValueProp.Move)
	};

	public ShadowStrike()
		: base(0, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		List<CardModel> hand = PileType.Hand.GetPile(base.Owner).Cards.ToList();
		IEnumerable<CardModel> selected = await CardSelectCmd.FromHandForDiscard(
			choiceContext,
			base.Owner,
			new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, minCount: 0, hand.Count),
			null,
			this);

		int discarded = 0;
		foreach (CardModel card in selected)
		{
			await CardCmd.Discard(choiceContext, card);
			discarded++;
		}

		if (discarded > 0)
		{
			await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
				.WithHitCount(discarded)
				.FromCard(this)
				.TargetingRandomOpponents(base.CombatState)
				.WithHitFx("vfx/vfx_attack_slash")
				.Execute(choiceContext);
		}
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Damage.UpgradeValueBy(2m);
	}
}
