using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Cards;
using Shadowfall.ShadowfallCode.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class CosmicForm : ShadowDefectCard
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Ethereal };

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		base.EnergyHoverTip
	};

	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new EnergyVar(0)
	};

	public CosmicForm()
		: base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await PowerCmd.Apply<CosmicFormPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, 1m, base.Owner.Creature, this);
	}

	protected override void OnUpgrade()
	{
		RemoveKeyword(CardKeyword.Ethereal);
	}
}
