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

    private readonly Dictionary<CardPlay, int> ResolvedCardCosts = [];

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner)
            return Task.CompletedTask;
        if (cardPlay.Card.EnergyCost.GetResolved() < DynamicVars.Energy.IntValue)
            return Task.CompletedTask;

        ResolvedCardCosts[cardPlay] = cardPlay.Card.EnergyCost.GetResolved();

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!ResolvedCardCosts.ContainsKey(cardPlay)) { return; }

        Flash();

        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        await PowerCmd.Apply<StrengthPower>(
            choiceContext, Owner,
            DynamicVars[StrengthKey].BaseValue,
            Owner, null);

        if (cardPlay.IsLastInSeries)
        {
            ResolvedCardCosts.Remove(cardPlay);
        }
    }
}
