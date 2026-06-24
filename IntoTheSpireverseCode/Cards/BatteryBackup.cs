using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Cards;

public sealed class BatteryBackup : ShadowDefectCard
{
	public override IEnumerable<CardKeyword> CanonicalKeywords => new[] { CardKeyword.Exhaust };

	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
	{
		new EnergyVar(0)
	};

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		base.EnergyHoverTip
	};

	public BatteryBackup()
		: base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

		// TODO: confirm the property name for max energy per turn on Owner or PlayerCombatState
		int maxEnergy = base.Owner.MaxEnergy;
		int currentEnergy = base.Owner.PlayerCombatState.Energy;
		int bonusEnergy = base.IsUpgraded ? base.DynamicVars.Energy.IntValue : 0;

		int toGain = (maxEnergy - currentEnergy) + bonusEnergy;
		if (toGain > 0)
			await PlayerCmd.GainEnergy(toGain, base.Owner);
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.Energy.UpgradeValueBy(1m);
	}
}
