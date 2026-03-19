using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Shadowfall.Orbs;
using Shadowfall.ShadowfallCode.Cards;

namespace Shadowfall.ShadowfallCode.Cards;

public sealed class Expansion : ShadowDefectCard
{
	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new RepeatVar(1)
	};

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.Static(StaticHoverTip.Channeling),
		HoverTipFactory.FromOrb<EntropyOrb>()
	};

	public Expansion()
		: base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		await OrbCmd.AddSlots(base.Owner, base.DynamicVars.Repeat.IntValue);
		await OrbCmd.Channel<EntropyOrb>(choiceContext, base.Owner);
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Repeat.UpgradeValueBy(1m);
	}
}
