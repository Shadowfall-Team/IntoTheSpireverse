using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>Deliberately not capped per turn - each separate instance of HP loss counts.</summary>
public sealed class ClaySoldierPower : ShadowPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(0m, ValueProp.Unpowered),
    ];

    public override int DisplayAmount => DynamicVars.Block.IntValue;

    public void AddVars(decimal block)
    {
        AssertMutable();
        DynamicVars.Block.BaseValue += block;
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext,
        Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0) return;

        Flash();
        await PowerCmd.Apply<BlockNextTurnPower>(
            choiceContext,
            Owner, DynamicVars.Block.BaseValue, Owner, null);
    }
}
