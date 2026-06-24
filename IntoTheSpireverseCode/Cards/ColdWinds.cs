using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards;

public sealed class ColdWinds : ShadowDefectCard
{
	private static readonly MethodInfo _evokeMethod = typeof(OrbCmd).GetMethod(
		"Evoke",
		BindingFlags.NonPublic | BindingFlags.Static,
		new[] { typeof(PlayerChoiceContext), typeof(MegaCrit.Sts2.Core.Entities.Players.Player), typeof(OrbModel), typeof(bool) })!;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.Static(StaticHoverTip.Evoke),
		HoverTipFactory.Static(StaticHoverTip.Channeling),
		HoverTipFactory.FromOrb<FrostOrb>()
	};

	public ColdWinds()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	private static Task EvokeOrb(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player, OrbModel orb, bool dequeue = true)
	{
		return (Task)_evokeMethod.Invoke(null, new object[] { choiceContext, player, orb, dequeue })!;
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

		List<OrbModel> toEvoke = base.Owner.PlayerCombatState.OrbQueue.Orbs
			.Where(o => o is not FrostOrb)
			.ToList();

		foreach (OrbModel orb in toEvoke)
		{
			await EvokeOrb(choiceContext, base.Owner, orb, dequeue: true);
			await Cmd.CustomScaledWait(0.1f, 0.25f);
		}

		int frostCount = toEvoke.Count + 1;
		for (int i = 0; i < frostCount; i++)
			await OrbCmd.Channel<FrostOrb>(choiceContext, base.Owner);
	}

	protected override void OnUpgrade()
	{
		AddKeyword(CardKeyword.Retain);
	}
}