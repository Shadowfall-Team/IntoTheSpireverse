using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

public sealed class JoltPower : ShadowPowerModel
{
    // Tracked per side, so Jolt can trigger once during the player's turn and once during the enemy's.
    private bool _triggeredOnPlayerTurn;
    private bool _triggeredOnEnemyTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (Owner.CombatState == null || Owner.Player == null || target != Owner || result.UnblockedDamage <= 0)
            return;

        var onEnemyTurn = Owner.CombatState.CurrentSide == CombatSide.Enemy;
        if (onEnemyTurn ? _triggeredOnEnemyTurn : _triggeredOnPlayerTurn)
            return;

        if (onEnemyTurn)
            _triggeredOnEnemyTurn = true;
        else
            _triggeredOnPlayerTurn = true;

        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side == CombatSide.Enemy)
            _triggeredOnEnemyTurn = false;
        else if (side == Owner.Side)
            _triggeredOnPlayerTurn = false;

        return Task.CompletedTask;
    }
}