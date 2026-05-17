using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;
using Shadowfall.ShadowfallCode.Cards;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class TheLaw : ShadowDefectCard
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
			base.DynamicVars.Cards.BaseValue = _clawCount;
		}
	}
	
	
	[SavedProperty]
	public int ExtraCount
	{
		get => this._extraCount;
		set
		{
			this.AssertMutable();
			this._extraCount = value;
		}
	}
	
	public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new CardsVar(ClawCount),
		(DynamicVar) new IntVar("Increase", 1M)
	};

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.FromCard<Claw>(this.IsUpgraded)
	};

	public TheLaw()
		: base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

		List<CardModel> generated = new List<CardModel>();
		for (int i = 0; i < ClawCount; i++)
		{
			CardModel claw = base.CombatState.CreateCard<Claw>(base.Owner);
			await CardPileCmd.AddGeneratedCardToCombat(claw, PileType.Hand, Owner);
			generated.Add(claw);
		}

		await Cmd.CustomScaledWait(0.0f, 0.25f);

		foreach (CardModel card in generated)
			CardCmd.Upgrade(card);

		BuffFromPlay();
		if (!(this.DeckVersion is TheLaw deckVersion))
			return;
		deckVersion.BuffFromPlay();
	}

	private void BuffFromPlay()
	{
		ExtraCount++;
		ClawCount = 1 + ExtraCount;
	}
}