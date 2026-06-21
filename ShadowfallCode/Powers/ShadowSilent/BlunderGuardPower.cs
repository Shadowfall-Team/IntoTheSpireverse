using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Shadowfall.ShadowfallCode.Powers.ShadowSilent;

public sealed class BlunderGuardPower : CustomPowerModel, IHasSecondAmount
{
    private const string StrengthKey = "StrengthGain";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(StrengthKey, 0m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<StrengthPower>(),
    ];

    public string GetSecondAmount() =>
        DynamicVars[StrengthKey].IntValue.ToString();

    public void AddStrength(int value)
    {
        AssertMutable();
        DynamicVars[StrengthKey].BaseValue += value;
        this.InvokeSecondAmountChanged();
    }

    public override async Task AfterCardPlayed(
        PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;
        if (cardPlay.Card.EnergyCost.GetResolved() < 3)
            return;

        Flash();

        await CreatureCmd.GainBlock(
            Owner, (decimal)Amount, ValueProp.Unpowered, null);

        await PowerCmd.Apply<StrengthPower>(
            new ThrowingPlayerChoiceContext(), Owner,
            DynamicVars[StrengthKey].BaseValue,
            Owner, null);
    }
}