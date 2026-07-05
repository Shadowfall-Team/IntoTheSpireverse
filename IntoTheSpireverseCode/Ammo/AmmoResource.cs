using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public static class AmmoResource
{
    private static readonly SpireField<PlayerCombatState, int> PlayerAmmo = new(() => 0);
    private static readonly SpireField<PlayerCombatState, CardModel?> PhantomShotCard = new(() => null);

    public static CardModel? GetOrCreatePhantomCard(Player player)
    {
        if (player.PlayerCombatState == null || player.Creature.CombatState == null) return null;
        return PhantomShotCard[player.PlayerCombatState] ??=
            player.Creature.CombatState.CreateCard<AmmoVolley>(player);
    }

    public static event Action<PlayerCombatState, int, int>? AmmoChanged;

    // TODO: stub for future ship muzzle-flash VFX
    // public static event Action<Player>? OnAmmoFiredStub;

    public static int GetAmmo(Player player) => player.PlayerCombatState != null ? PlayerAmmo[player.PlayerCombatState] : 0;

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


    public const decimal BaseDamage = 12;

    /// <summary>
    /// Base damage + Strength + IModifiesAmmoShotDamage listeners (Firepower, Volley Damage).
    /// </summary>
    public static decimal GetShotDamage(Player player)
    {
        var damage = BaseDamage;

        // Strength
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