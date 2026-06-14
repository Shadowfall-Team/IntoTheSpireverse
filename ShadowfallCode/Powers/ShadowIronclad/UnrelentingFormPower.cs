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
    // MAYBE: Investigate the rare circumstance where this activates, and then more stacks are added
    // Its probably fine to leave as is, since its the "First" time, and things like lethality don't refresh
    private class Data
    {
        public bool hasTriggeredThisTurn;
    }

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object? InitInternalData() { return new Data(); }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(0),
        new EnergyVar(0),
    ];

    public override int DisplayAmount => DynamicVars.Energy.IntValue;
    public string GetSecondAmount()
    {
        return DynamicVars.Cards.BaseValue.ToString();
    }

    public void AddVars(decimal cardDraw, decimal energy)
    {
        AssertMutable();
        DynamicVars.Cards.BaseValue += cardDraw;
        this.InvokeSecondAmountChanged();
        DynamicVars.Energy.BaseValue += energy;
        InvokeDisplayAmountChanged();
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.ForEnergy(this)];

    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (!(player.PlayerCombatState is { Phase: PlayerTurnPhase.Play }) || player != Owner.Player)
            return;

        var data = GetInternalData<Data>();
        if (data.hasTriggeredThisTurn)
            return;

        data.hasTriggeredThisTurn = true;
        Flash();
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner.Player);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner.Player);
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side)
            return Task.CompletedTask;

        GetInternalData<Data>().hasTriggeredThisTurn = false;
        return Task.CompletedTask;
    }
}
