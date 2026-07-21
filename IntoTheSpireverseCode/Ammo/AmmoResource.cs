using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public static class AmmoResource
{
    private static readonly SpireField<PlayerCombatState, int> PlayerAmmo = new(() => 0);
    private static readonly SpireField<PlayerCombatState, CardModel?> PhantomShotCard = new(() => null);
    private static readonly SpireField<PlayerCombatState, Creature?> LastAttackTarget = new(() => null);

    public static CardModel? GetOrCreatePhantomCard(Player player)
    {
        if (player.PlayerCombatState == null || player.Creature.CombatState == null) return null;
        return PhantomShotCard[player.PlayerCombatState] ??=
            player.Creature.CombatState.CreateCard<AmmoVolley>(player);
    }

    public static event Action<PlayerCombatState, int, int>? AmmoChanged;

    // TODO: stub for future ship muzzle-flash VFX
    // public static event Action<Player>? OnAmmoFiredStub;

    public static int GetAmmo(Player player) =>
        player.PlayerCombatState != null ? PlayerAmmo[player.PlayerCombatState] : 0;

    public static bool CanSpendAmmo(Player player)
    {
        if (player.PlayerCombatState == null) return false;
        if (GetAmmo(player) <= 0) return false;
        return player.PlayerCombatState.Energy >= GetShotEnergyCost(player);
    }

    public static async Task GainAmmo(int amount, Player player)
    {
        if (player.PlayerCombatState == null || player.Creature.CombatState == null) return;

        for (var i = 0; i < amount; i++)
        {
            var oldVal = PlayerAmmo[player.PlayerCombatState];
            PlayerAmmo[player.PlayerCombatState] = oldVal + 1;
            AmmoChanged?.Invoke(player.PlayerCombatState, oldVal, oldVal + 1);

            foreach (var model in player.Creature.CombatState.IterateHookListeners().ToList())
            {
                if (model is IAmmoLoadedListener listener)
                    await listener.OnAmmoLoaded();
            }
        }
    }

    internal static void LoseAmmo(int amount, Player player)
    {
        if (player.PlayerCombatState == null) return;
        var oldVal = PlayerAmmo[player.PlayerCombatState];
        var newVal = Math.Max(0, oldVal - amount);
        if (newVal == oldVal) return;
        PlayerAmmo[player.PlayerCombatState] = newVal;
        AmmoChanged?.Invoke(player.PlayerCombatState, oldVal, newVal);
    }


    /// <summary>
    /// True while a shot's damage is resolving. Ammo damage is dealt as plain Unpowered damage,
    /// so this is the only way for a power to tell it apart from other non-attack damage.
    /// Shots resolve one at a time on the action queue, so a simple flag is sufficient.
    /// </summary>
    public static bool IsResolvingShot { get; private set; }

    internal static async Task WhileResolvingShot(Func<Task> resolve)
    {
        IsResolvingShot = true;
        try
        {
            await resolve();
        }
        finally
        {
            IsResolvingShot = false;
        }
    }

    public static void SetLastAttackTarget(Player player, Creature target)
    {
        if (player.PlayerCombatState == null) return;
        LastAttackTarget[player.PlayerCombatState] = target;
    }

    public static Creature? GetLastAttackTarget(Player player)
    {
        if (player.PlayerCombatState == null) return null;
        var target = LastAttackTarget[player.PlayerCombatState];
        if (target == null) return null;
        return player.Creature.CombatState?.HittableEnemies.Contains(target) == true ? target : null;
    }

    public static Creature? PickShotTarget(Player player, ICombatState combatState)
    {
        var lastTarget = GetLastAttackTarget(player);
        if (lastTarget != null) return lastTarget;

        return player.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
    }


    public const decimal BaseDamage = 12;

    public static decimal GetShotDamage(Player player)
    {
        var damage = BaseDamage;

        if(player.HasPower<AmmoStrengthPower>())
            damage += player.Creature.GetPowerAmount<StrengthPower>();

        // Firepower, Volley, and any future IModifiesAmmoShotDamage powers
        foreach (var model in player.Creature.CombatState!.IterateHookListeners())
        {
            if (model is IModifiesAmmoShotDamage modifier)
                damage = modifier.ModifyAmmoShotDamage(player, damage);
        }

        return damage;
    }

    public static int GetShotEnergyCost(Player player)
    {
        var cost = 1;
        foreach (var model in player.Creature.CombatState!.IterateHookListeners())
        {
            if (model is IModifiesShotCost modifier)
                cost = modifier.ModifyShotCost(cost);
        }

        return cost;
    }

    /// <summary>
    /// Resolves one Ammo shot: spends the Ammo, fires the missile and runs every on-fire
    /// effect. Shared by the FIRE button and by cards that fire on the player's behalf.
    /// Returns false if the shot could not happen at all (no Ammo, no energy, no targets).
    /// </summary>
    public static async Task<bool> TryFireShot(Player player, bool chargeEnergy)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return false;

        var cost = chargeEnergy ? GetShotEnergyCost(player) : 0;
        var hasBigGuns = player.Creature.HasPower<MassMunitionPower>();

        if (GetAmmo(player) <= 0 || player.PlayerCombatState?.Energy < cost ||
            !hasBigGuns && !combatState.HittableEnemies.Any())
        {
            return false;
        }

        if (chargeEnergy)
        {
            await PlayerCmd.LoseEnergy(cost, player);
            var phantomCard = GetOrCreatePhantomCard(player);
            if (phantomCard != null)
                await Hook.AfterEnergySpent(combatState, phantomCard, cost);
        }

        LoseAmmo(1, player);
        await InvokeOnAmmoFiring(player);

        Creature? pickedTarget = null;
        if (!hasBigGuns)
            pickedTarget = PickShotTarget(player, combatState);

        await ShotHelper.CreateMissile(combatState, pickedTarget);

        var blockAmount = combatState.IterateHookListeners()
            .OfType<DefensiveCannonadePower>()
            .Where(p => p.Owner == player.Creature)
            .Sum(p => p.Amount);
        if (blockAmount > 0)
        {
            await CreatureCmd.GainBlock(player.Creature, blockAmount, ValueProp.Move, null);
        }

        var shotDamage = GetShotDamage(player);
        IEnumerable<Creature> targets = hasBigGuns
            ? combatState.HittableEnemies
            : (IEnumerable<Creature>)[pickedTarget!];

        IEnumerable<DamageResult> results = [];
        await WhileResolvingShot(async () =>
        {
            results = await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                targets, shotDamage, ValueProp.Unpowered, player.Creature);

            if (!player.Creature.HasPower<GrapeshotPower>()) return;

            var grapeshot = player.Creature.GetPowerAmount<GrapeshotPower>();
            var halfDmg = Math.Floor(0.5m * GetShotDamage(player));
            for (var i = 0; i < grapeshot; i++)
            {
                if (hasBigGuns)
                {
                    await ShotHelper.CreateMissile(combatState, null, skipWait: true);
                    foreach (var t in combatState.HittableEnemies)
                        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                            t, halfDmg, ValueProp.Unpowered, player.Creature);
                }
                else
                {
                    var followTarget = PickShotTarget(player, combatState);
                    await ShotHelper.CreateMissile(combatState, followTarget, skipWait: true);
                    if (followTarget == null) continue;
                    await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(),
                        followTarget, halfDmg, ValueProp.Unpowered, player.Creature);
                }
            }
        });

        await InvokeOnAmmoFired(player, [results.ToList()]);
        return true;
    }

    public static async Task InvokeOnAmmoFiring(Player player)
    {
        foreach (var model in player.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IAmmoFiringListener listener)
                await listener.OnAmmoFiring(player);
        }
    }

    public static async Task InvokeOnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        // OnAmmoFiredStub?.Invoke(player);
        var resultList = results.ToList();
        foreach (var model in player.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IAmmoFiredListener listener)
                await listener.OnAmmoFired(player, resultList);
        }
    }
}