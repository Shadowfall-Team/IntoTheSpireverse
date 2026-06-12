using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class UnrelentingFormPower : CustomPowerModel, IHasSecondAmount
{
    private static readonly bool _allAtOnce = true;
    private class Data
    {
        public int timesTriggeredThisTurn;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object? InitInternalData()
    {
        return new Data();
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("CardDraw", 0m),
        new EnergyVar(0),
    ];

    public override int DisplayAmount => _allAtOnce ? DynamicVars.Energy.IntValue : Amount;
    public string GetSecondAmount()
    {
        return _allAtOnce ? DynamicVars["CardDraw"].BaseValue.ToString() : "";
    }

    public void AddVars(decimal cardDraw, decimal energy)
    {
        AssertMutable();
        DynamicVars["CardDraw"].BaseValue += cardDraw;
        this.InvokeSecondAmountChanged();
        DynamicVars.Energy.BaseValue += energy;
        InvokeDisplayAmountChanged();
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (!(player.PlayerCombatState is { Phase: PlayerTurnPhase.Play }) || player != Owner.Player)
            return;

        var data = GetInternalData<Data>();
        if (data.timesTriggeredThisTurn > Amount || (_allAtOnce && data.timesTriggeredThisTurn > 0))
            return;

        GetInternalData<Data>().timesTriggeredThisTurn++;
        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner.Player);
        await CardPileCmd.Draw(choiceContext, DynamicVars["CardDraw"].BaseValue, Owner.Player);
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
            return Task.CompletedTask;

        GetInternalData<Data>().timesTriggeredThisTurn = 0;
        return Task.CompletedTask;
    }
}
