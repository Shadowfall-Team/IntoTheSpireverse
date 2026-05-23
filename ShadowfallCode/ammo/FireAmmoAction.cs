using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;
using Shadowfall.ShadowfallCode.Powers.ShadowRegent;

namespace Shadowfall.ShadowfallCode.ammo;

public class FireAmmoAction : GameAction
{
    private readonly Player _player;

    public override ulong OwnerId => _player.NetId;
    public override GameActionType ActionType => GameActionType.CombatPlayPhaseOnly;

    public FireAmmoAction(Player player)
    {
        _player = player;
    }

    protected override async Task ExecuteAction()
    {
        // Validate
        if (AmmoResource.GetAmmo(_player) <= 0)
        {
            Cancel();
            return;
        }
        if (_player.PlayerCombatState.Energy < 1)
        {
            Cancel();
            return;
        }

        // Spend resources
        AmmoResource.LoseAmmo(1, _player);
        await PlayerCmd.LoseEnergy(1, _player);

        // Calculate damage from phantom card
        var ammoState = AmmoResource.GetOrCreateState(_player);
        var phantomCard = ammoState.PhantomCard;
        var damage = phantomCard.DynamicVars.CalculatedDamage;

        // Determine targets
        var hasBigGuns = _player.Creature.HasPower<BigGunsPower>();
        IReadOnlyList<Creature> targets = hasBigGuns
            ? _player.Creature.CombatState.Enemies.Where(e => e.IsAlive).ToList()
            : _player.Creature.CombatState.Enemies.Where(e => e.IsAlive).Take(1).ToList();

        // Build attack command
        var command = DamageCmd.Attack(damage)
            .WithHitCount(1)
            .FromCard(phantomCard)
            .WithAttackerAnim("Cast", _player.Character.AttackAnimDelay)
            .WithAttackerFx(null, "event:/sfx/characters/regent/regent_sovereign_blade", null);

        if (hasBigGuns)
        {
            command.TargetingAllOpponents(_player.Creature.CombatState);
        }
        else
        {
            command.TargetingRandomOpponents(_player.Creature.CombatState);
        }

        await command.Execute(new ThrowingPlayerChoiceContext());

        // Fire event for ShellVolleySingleton and other subscribers
        AmmoResource.FireOnAmmoFired(_player, targets);
    }

    public override INetAction ToNetAction()
    {
        return new NetFireAmmoAction();
    }

    public override string ToString()
    {
        return $"FireAmmoAction for player {_player.NetId}";
    }
}
