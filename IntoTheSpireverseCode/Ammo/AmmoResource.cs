using BaseLib.Extensions;
using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards.Colorless;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

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


    public static event Action<PlayerCombatState, Creature?>? LastAttackTargetChanged;

    public static void SetLastAttackTarget(Player player, Creature target)
    {
        if (player.PlayerCombatState == null) return;
        if (LastAttackTarget[player.PlayerCombatState] == target) return;
        LastAttackTarget[player.PlayerCombatState] = target;
        LastAttackTargetChanged?.Invoke(player.PlayerCombatState, target);
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

    // Shot resolution lives in FireAmmoCmd. This type owns the Ammo resource itself: the count,
    // the target it remembers, and the numbers a shot is worth.

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