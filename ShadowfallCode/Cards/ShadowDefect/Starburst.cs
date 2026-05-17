using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.Orbs;
using Shadowfall.ShadowfallCode.Cards;
using Shadowfall.ShadowfallCode.Powers;

namespace Shadowfall.ShadowfallCode.Cards.ShadowDefect;

public sealed class Starburst : ShadowDefectCard
{
	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new CardsVar(2)
	};

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.Static(StaticHoverTip.Evoke),
		HoverTipFactory.FromOrb<EntropyOrb>(),
		HoverTipFactory.FromCard<Shiv>()
	};

	public Starburst()
		: base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		if (base.IsUpgraded)
			await OrbCmd.Channel<EntropyOrb>(choiceContext, base.Owner);
		await PowerCmd.Apply<StarburstPower>(new ThrowingPlayerChoiceContext(), base.Owner.Creature, base.DynamicVars.Cards.BaseValue, base.Owner.Creature, this);
	}
}
