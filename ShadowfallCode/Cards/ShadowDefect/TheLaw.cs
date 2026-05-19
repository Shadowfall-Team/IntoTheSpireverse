using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class TheLaw() : ShadowDefectCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
	private const string _increaseKey = "Increase";
	private int _clawCount = 1;
	private int _extraCount;

	[SavedProperty]
	public int ClawCount
	{
		get => _clawCount;
		set
		{
			AssertMutable();
			_clawCount = value;
			DynamicVars.Cards.BaseValue = _clawCount;
		}
	}
	
	
	[SavedProperty]
	public int ExtraCount
	{
		get => _extraCount;
		set
		{
			AssertMutable();
			_extraCount = value;
		}
	}
	
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new CardsVar(ClawCount),
		new IntVar("Increase", 1)
	];

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
	[
		HoverTipFactory.FromCard<Claw>(IsUpgraded)
	];

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

		List<CardModel> generated = new List<CardModel>();
		for (int i = 0; i < ClawCount; i++)
		{
			CardModel claw = CombatState!.CreateCard<Claw>(Owner);
			await CardPileCmd.AddGeneratedCardToCombat(claw, PileType.Hand, Owner);
			generated.Add(claw);
		}

		await Cmd.CustomScaledWait(0.0f, 0.25f);

		foreach (CardModel card in generated)
			CardCmd.Upgrade(card);

		BuffFromPlay();
		if (DeckVersion is not TheLaw deckVersion)
			return;
		deckVersion.BuffFromPlay();
	}

	private void BuffFromPlay()
	{
		ExtraCount++;
		ClawCount = 1 + ExtraCount;
	}
}