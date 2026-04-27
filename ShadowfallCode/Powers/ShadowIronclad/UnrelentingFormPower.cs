using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

public sealed class UnrelentingFormPower : CustomPowerModel
{
    private bool _triggeredThisTurn;
    private const int DrawCount = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.ForEnergy(this)
    ];

    public override async Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)
    {
        if (!(player.PlayerCombatState is { Phase: PlayerTurnPhase.Play }) || player != Owner.Player || _triggeredThisTurn)
            return;

        _triggeredThisTurn = true;
        Flash();
        await PlayerCmd.GainEnergy((decimal)Amount, Owner.Player);
        await CardPileCmd.Draw(choiceContext, DrawCount, Owner.Player);
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        ICombatState combatState)
    {
        if (side != Owner.Side)
            return Task.CompletedTask;

        _triggeredThisTurn = false;
        return Task.CompletedTask;
    }
}