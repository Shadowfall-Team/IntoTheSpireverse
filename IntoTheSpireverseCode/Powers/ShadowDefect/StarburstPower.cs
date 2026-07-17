using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.Orbs;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowDefect;

// TODO : description and smartdescription
public sealed class StarburstPower : IntoTheSpireversePower
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips =>
	[
		HoverTipFactory.Static(StaticHoverTip.Evoke),
		HoverTipFactory.FromOrb<EntropyOrb>(),
		HoverTipFactory.FromCard<Shiv>()
	];

	public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)
	{
		if (orb.Owner == base.Owner.Player && orb is EntropyOrb)
		{
			Flash();
			await Shiv.CreateInHand(base.Owner.Player, base.Amount, base.CombatState);
		}
	}
}
