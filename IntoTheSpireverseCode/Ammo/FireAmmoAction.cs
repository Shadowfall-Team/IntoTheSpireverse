using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

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
        var combatState = _player.Creature.CombatState;
        if (combatState == null) { Cancel(); return; }

        var cost = AmmoResource.GetShotEnergyCost(_player);
        var hasBigGuns = _player.Creature.HasPower<BigGunsPower>();

        if (AmmoResource.GetAmmo(_player) <= 0 || _player.PlayerCombatState.Energy < cost ||
            !hasBigGuns && !combatState.HittableEnemies.Any())
        {
            Cancel();
            return;
        }

        await PlayerCmd.LoseEnergy(cost, _player);
        await Hook.AfterEnergySpent(combatState, AmmoResource.GetOrCreatePhantomCard(_player), cost);
        AmmoResource.LoseAmmo(1, _player);
        await AmmoResource.InvokeOnAmmoFiring(_player);

        Creature? pickedTarget = null;
        if (!hasBigGuns)
        {
            var hittableEnemies = combatState.HittableEnemies.ToList();
            var preferredTargets = hittableEnemies.Where(e => e.HasPower<TargetedPower>()).ToList();
            var targetPool = preferredTargets.Count > 0 ? preferredTargets : hittableEnemies;
            pickedTarget = _player.RunState.Rng.CombatTargets.NextItem(targetPool);
        }

        await ShotHelper.CreateMissile(combatState, pickedTarget);

        var blockAmount = combatState.IterateHookListeners()
            .OfType<DefensiveCannonadePower>()
            .Where(p => p.Owner == _player.Creature)
            .Sum(p => p.Amount);
        if (blockAmount > 0)
        {
            await CreatureCmd.GainBlock(_player.Creature, blockAmount, ValueProp.Move, null);
        }

        var shotDamage = AmmoResource.GetShotDamage(_player);
        IEnumerable<Creature> targets = hasBigGuns
            ? combatState.HittableEnemies
            : (IEnumerable<Creature>)[pickedTarget!];

        var results = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
            targets, shotDamage, ValueProp.Unpowered, _player.Creature);

        if (_player.Creature.HasPower<GrapeshotPower>())
        {
            var grapeshot = _player.Creature.GetPowerAmount<GrapeshotPower>();
            var halfDmg = Math.Floor(0.5m * AmmoResource.GetShotDamage(_player));
            for (var i = 0; i < grapeshot; i++)
            {
                if (hasBigGuns)
                {
                    await ShotHelper.CreateMissile(combatState, null, skipWait: true);
                    foreach (var t in combatState.HittableEnemies)
                        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                            t, halfDmg, ValueProp.Unpowered, _player.Creature);
                }
                else
                {
                    var hittableEnemies = combatState.HittableEnemies.ToList();
                    var preferredTargets = hittableEnemies.Where(e => e.HasPower<TargetedPower>()).ToList();
                    var targetPool = preferredTargets.Count > 0 ? preferredTargets : hittableEnemies;
                    var followTarget = _player.RunState.Rng.CombatTargets.NextItem(targetPool);

                    await ShotHelper.CreateMissile(combatState, followTarget, skipWait: true);
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                        followTarget, halfDmg, ValueProp.Unpowered, _player.Creature);
                }
            }
        }

        await AmmoResource.InvokeOnAmmoFired(_player, [results.ToList()]);
    }

    public override INetAction ToNetAction() => new NetFireAmmoAction();
    public override string ToString() => $"FireAmmoAction for player {_player.NetId}";
}
