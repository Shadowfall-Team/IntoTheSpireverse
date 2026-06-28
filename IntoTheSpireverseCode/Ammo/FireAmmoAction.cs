using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        if (combatState == null)
        {
            Cancel();
            return;
        }

        var ammoState = AmmoResource.GetOrCreateState(_player);
        var cost = AmmoResource.GetShotEnergyCost(_player);
        var hasBigGuns = _player.Creature.HasPower<BigGunsPower>();

        if (ammoState.Ammo <= 0 || _player.PlayerCombatState.Energy < cost ||
            !hasBigGuns && !combatState.HittableEnemies.Any())
        {
            Cancel();
            return;
        }

        await PlayerCmd.LoseEnergy(cost, _player);
        AmmoResource.LoseAmmo(1, _player);
        await AmmoResource.InvokeOnAmmoFiring(_player);

        var phantom = ammoState.PhantomCard;

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

        var attackCmd = DamageCmd.Attack(phantom.DynamicVars.Damage.BaseValue)
            .WithHitCount(1)
            .FromCard(phantom)
            .WithAttackerAnim("Cast", _player.Character.CastAnimDelay)
            .WithHitFx("vfx/vfx_starry_impact", null, "blunt_attack.mp3");

        attackCmd = hasBigGuns
            ? attackCmd.TargetingAllOpponents(combatState)
            : attackCmd.Targeting(pickedTarget!);

        var executedCmd = await attackCmd.Execute(new ThrowingPlayerChoiceContext());

        if (_player.Creature.HasPower<GrapeshotPower>())
        {
            var grapeshot = _player.Creature.GetPowerAmount<GrapeshotPower>();
            var baseDamage = phantom.DynamicVars.Damage.BaseValue;
            for (var i = 0; i < grapeshot; i++)
            {
                if (hasBigGuns)
                {
                    var followTargets = combatState.HittableEnemies;
                    await ShotHelper.CreateMissile(combatState, null, skipWait: true);
                    foreach (var t in followTargets)
                    {
                        var halfDmg = Math.Floor(0.5m * Hook.ModifyDamage(_player.RunState,
                            combatState,
                            t,
                            _player.Creature,
                            baseDamage,
                            ValueProp.Move,
                            phantom,
                            ModifyDamageHookType.Additive |
                            ModifyDamageHookType.Multiplicative, CardPreviewMode.None,
                            out _
                        ));
                        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), t,
                            halfDmg, ValueProp.Unpowered, _player.Creature, phantom);
                    }
                }
                else
                {
                    var hittableEnemies = combatState.HittableEnemies.ToList();
                    var preferredTargets = hittableEnemies.Where(e => e.HasPower<TargetedPower>()).ToList();
                    var targetPool = preferredTargets.Count > 0 ? preferredTargets : hittableEnemies;
                    var followTarget = _player.RunState.Rng.CombatTargets.NextItem(targetPool);

                    await ShotHelper.CreateMissile(combatState, followTarget, skipWait: true);

                    var halfDmg = Math.Floor(0.5m * Hook.ModifyDamage(_player.RunState,
                        combatState,
                        followTarget,
                        _player.Creature,
                        baseDamage,
                        ValueProp.Move,
                        phantom,
                        ModifyDamageHookType.Additive | ModifyDamageHookType.Multiplicative,
                        CardPreviewMode.None,
                        out _
                    ));
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), followTarget,
                        halfDmg, ValueProp.Unpowered, _player.Creature, phantom);
                }
            }
        }

        await AmmoResource.InvokeOnAmmoFired(_player, executedCmd.Results);
    }

    public override INetAction ToNetAction() => new NetFireAmmoAction();
    public override string ToString() => $"FireAmmoAction for player {_player.NetId}";
}