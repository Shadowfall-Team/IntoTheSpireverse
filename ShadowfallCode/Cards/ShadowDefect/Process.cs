using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class Process() : ShadowDefectCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
	[
		HoverTipFactory.FromCard<Void>()
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

		List<CardModel> cards = PileType.Draw.GetPile(Owner).Cards.ToList();

		CardModel? selected = (await CardSelectCmd.FromSimpleGrid(
			choiceContext,
			cards,
			Owner,
			new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();

		if (selected == null)
			return;

		int cost = Math.Max(0, selected.EnergyCost.GetWithModifiers(CostModifiers.All));

		await CardCmd.AutoPlay(choiceContext, selected, null);

		for (int i = 0; i < cost; i++)
		{
			CardModel? voidCard = CombatState?.CreateCard<Void>(Owner);
			CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(voidCard!, PileType.Discard, Owner));
		}
	}

	//TODO temp upgrade as not on doc
	protected override void OnUpgrade()
	{
		EnergyCost.UpgradeBy(-1);
	}
}
