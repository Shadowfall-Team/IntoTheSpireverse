using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Shadowfall.ShadowfallCode.Powers.ShadowSilent;

namespace Shadowfall.ShadowfallCode.Relics;

  
public class SneckoReplacement : ShadowSilentRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VigorPower>(1M),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
      HoverTipFactory.FromPower<BleedPower>(),
      HoverTipFactory.FromPower<VigorPower>()

    ];

    public override async Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
    {

	    if (applier == Owner.Creature && power is BleedPower)
	    {
		    await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars[nameof(VigorPower)].BaseValue, Owner.Creature, null);
	    }

	    await Task.CompletedTask;
    }

}