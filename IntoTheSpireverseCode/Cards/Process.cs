using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards;

public sealed class Process() : ShadowDefectCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.FromCard<Void>()
	};

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

		List<CardModel> cards = PileType.Draw.GetPile(base.Owner).Cards.ToList();

		CardModel? selected = (await CardSelectCmd.FromSimpleGrid(
			choiceContext,
			cards,
			base.Owner,
			new CardSelectorPrefs(base.SelectionScreenPrompt, 1))).FirstOrDefault();

		if (selected == null)
			return;

		int cost = Math.Max(0, selected.EnergyCost.GetWithModifiers(CostModifiers.All));

		await CardCmd.AutoPlay(choiceContext, selected, null);

		for (int i = 0; i < cost; i++)
		{
			CardModel voidCard = base.CombatState.CreateCard<Void>(base.Owner);
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(voidCard, PileType.Discard, Owner));
		}
	}

	protected override void OnUpgrade()
	{
		base.EnergyCost.UpgradeBy(-1);
	}
}
