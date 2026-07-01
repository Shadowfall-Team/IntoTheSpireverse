using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public static class AmmoResource
{
    private static readonly SpireField<PlayerCombatState, int> _playerAmmo = new(() => 0);
    private static readonly SpireField<PlayerCombatState, CardModel?> _phantomShotCard = new(() => null);

    public static CardModel GetOrCreatePhantomCard(Player player)
    {
        return _phantomShotCard[player.PlayerCombatState] ??=
            player.Creature.CombatState!.CreateCard<AmmoVolley>(player);
    }

    public static event Action<PlayerCombatState, int, int>? AmmoChanged;

    // TODO: stub for future ship muzzle-flash VFX
    // public static event Action<Player>? OnAmmoFiredStub;

    public static int GetAmmo(Player player) => _playerAmmo[player.PlayerCombatState];

    public static async Task GainAmmo(int amount, Player player)
    {
        if (player.Creature.CombatState == null) return;

        for (var i = 0; i < amount; i++)
        {
            var oldVal = _playerAmmo[player.PlayerCombatState];
            _playerAmmo[player.PlayerCombatState] = oldVal + 1;
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
        var oldVal = _playerAmmo[player.PlayerCombatState];
        var newVal = Math.Max(0, oldVal - amount);
        if (newVal == oldVal) return;
        _playerAmmo[player.PlayerCombatState] = newVal;
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

        foreach (var model in player.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IAmmoFiredListener listener)
                await listener.OnAmmoFired(player, results);
        }
    }
}