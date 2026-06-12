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

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public class PillarOfMutationPower : CustomPowerModel, IHasSecondAmount
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(0m, ValueProp.Unpowered),
        new PowerVar<VigorPower>(1m),
    ];

    // just in case someone modifies dynamic vars lol
    public override int DisplayAmount => DynamicVars.Power<VigorPower>().IntValue * Amount;

    public void AddBlock(decimal block)
    {
        AssertMutable();
        DynamicVars.Block.BaseValue += block;
        this.InvokeSecondAmountChanged();
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != null && creator == Owner.Player)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner, DynamicVars.Block, null, fast: true);
            await PowerCmd.Apply<VigorPower>(
                new ThrowingPlayerChoiceContext(),
                Owner, DynamicVars.Power<VigorPower>().BaseValue * Amount, Owner, null
            );
        }
    }

    public string GetSecondAmount()
    {
        return DynamicVars.Block.BaseValue.ToString();
    }
}
