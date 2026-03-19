using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Shadowfall.ShadowfallCode.Powers;

public sealed class SongOfSorrowPower : CustomPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Counter;

	protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
	{
		HoverTipFactory.FromCard<Void>()
	};

	public override async Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
	{
		if (addedByPlayer && card.Owner == base.Owner.Player && card is Void)
		{
			Flash();
			await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), base.CombatState.HittableEnemies, base.Amount, ValueProp.Unblockable | ValueProp.Unpowered, base.Owner, null);
		}
	}
}
