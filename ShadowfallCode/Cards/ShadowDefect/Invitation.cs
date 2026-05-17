using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards;

namespace Shadowfall.ShadowfallCode.Cards;

public sealed class Invitation : ShadowDefectCard
{
	public override bool GainsBlock => true;

	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new BlockVar(3m, ValueProp.Move),
		new CardsVar(1)
	};

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.Static(StaticHoverTip.Block)
	};

	public Invitation()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);

		List<CardModel> candidates = PileType.Draw.GetPile(base.Owner).Cards
			.Concat(PileType.Discard.GetPile(base.Owner).Cards)
			.Where(c => c.Type == CardType.Status || c.Type == CardType.Curse)
			.ToList();

		if (candidates.Count == 0)
			return;

		IEnumerable<CardModel> selected = await CardSelectCmd.FromSimpleGrid(
			choiceContext,
			candidates,
			base.Owner,
			new CardSelectorPrefs(base.SelectionScreenPrompt, base.DynamicVars.Cards.IntValue));

		await CardPileCmd.Add(selected, PileType.Hand);
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(1m);
		base.DynamicVars.Cards.UpgradeValueBy(1m);
	}
}
