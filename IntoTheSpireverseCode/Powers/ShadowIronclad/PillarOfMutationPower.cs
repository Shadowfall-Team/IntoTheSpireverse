using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowIronclad;

public class PillarOfMutationPower : ShadowPowerModel, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(0m, ValueProp.Unpowered),
        new PowerVar<VigorPower>(0m),
    ];

    // use the var instead of amount, just in case someone modifies dynamic vars
    public override int DisplayAmount => DynamicVars.Power<VigorPower>().IntValue;
    public string GetSecondAmount() => DynamicVars.Block.BaseValue.ToString();

    public void AddVars(decimal block, decimal vigor)
    {
        AssertMutable();
        DynamicVars.Block.BaseValue += block;
        this.InvokeSecondAmountChanged();
        DynamicVars.Power<VigorPower>().BaseValue += vigor;
        InvokeDisplayAmountChanged();
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != null && creator == Owner.Player)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, DynamicVars.Block, null, fast: true);
            await PowerCmd.Apply<VigorPower>(new ThrowingPlayerChoiceContext(),
                Owner, DynamicVars.Power<VigorPower>().BaseValue, Owner, null);
        }
    }
}
