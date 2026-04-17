using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class BerserkingPower : CustomPowerModel
{
    private const string SelfDamageKey = "SelfDamage";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(SelfDamageKey, 0m, ValueProp.Unblockable | ValueProp.Unpowered)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;

        Flash();

        DamageVar selfDamage = (DamageVar)DynamicVars[SelfDamageKey];
        await CreatureCmd.Damage(choiceContext, Owner, selfDamage.BaseValue, selfDamage.Props, Owner, (CardModel?)null);
        await PlayerCmd.GainEnergy((decimal)Amount, Owner.Player);
    }

    public void IncrementSelfDamage()
    {
        AssertMutable();
        ++DynamicVars[SelfDamageKey].BaseValue;
    }
}