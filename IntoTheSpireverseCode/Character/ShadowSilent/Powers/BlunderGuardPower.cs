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

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Powers;

public class BlunderGuardPower : ShadowPowerModel, IHasSecondAmount
{
    private const string StrengthKey = "StrengthGain";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar(StrengthKey, 0m),
        new EnergyVar(3)
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

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return;
        if (cardPlay.Card.EnergyCost.GetResolved() < DynamicVars.Energy.IntValue)
            return;

        Flash();

        await CreatureCmd.GainBlock(
            Owner, Amount, ValueProp.Unpowered, null);

        // TODO: Replace this with HookPlayerChoiceContext when i bug pikcube enough
        await PowerCmd.Apply<StrengthPower>(
            new BlockingPlayerChoiceContext(), Owner,
            DynamicVars[StrengthKey].BaseValue,
            Owner, null);
    }
}