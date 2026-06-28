using BaseLib.Utils;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public static class AmmoResource
{
    private static readonly SpireField<PlayerCombatState, AmmoState?> _ammoState = new(() => null);

    public static AmmoState GetOrCreateState(Player player)
    {
        var pcs = player.PlayerCombatState;
        var ammoState = _ammoState[pcs];
        if (ammoState != null)
        {
            return ammoState;
        }

        var phantomCard = player.Creature.CombatState!.CreateCard<AmmoVolley>(player);
        ammoState = new AmmoState { Ammo = 0, PhantomCard = phantomCard };
        _ammoState[pcs] = ammoState;
        return ammoState;
    }

    public static event Action<PlayerCombatState, int, int>? AmmoChanged;

    // TODO: stub for future ship muzzle-flash VFX
    // public static event Action<Player>? OnAmmoFiredStub;

    public static int GetAmmo(Player player) => GetOrCreateState(player).Ammo;

    public static async Task GainAmmo(int amount, Player player)
    {
        if (player.Creature.CombatState == null) return;

        var state = GetOrCreateState(player);
        for (var i = 0; i < amount; i++)
        {
            var oldVal = state.Ammo;
            state.Ammo++;
            AmmoChanged?.Invoke(player.PlayerCombatState, oldVal, state.Ammo);

            foreach (var model in player.Creature.CombatState.IterateHookListeners().ToList())
            {
                if (model is IAmmoLoadedListener listener)
                {
                    await listener.OnAmmoLoaded();
                }
            }
        }
    }

    public static void LoseAmmo(int amount, Player player)
    {
        var ammoState = GetOrCreateState(player);
        var oldVal = ammoState.Ammo;
        ammoState.Ammo = Math.Max(0, ammoState.Ammo - amount);
        if (ammoState.Ammo != oldVal)
        {
            AmmoChanged?.Invoke(player.PlayerCombatState, oldVal, ammoState.Ammo);
        }
    }

    public static int GetShotEnergyCost(Player player)
    {
        var cost = 1;
        foreach (var model in player.Creature.CombatState!.IterateHookListeners())
        {
            if (model is IModifiesShotCost modifier)
            {
                cost = modifier.ModifyShotCost(cost);
            }
        }

        return cost;
    }

    public static async Task InvokeOnAmmoFiring(Player player)
    {
        foreach (var model in player.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IAmmoFiringListener listener)
            {
                await listener.OnAmmoFiring(player);
            }
        }
    }

    public static async Task InvokeOnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        // OnAmmoFiredStub?.Invoke(player);

        foreach (var model in player.Creature.CombatState!.IterateHookListeners().ToList())
        {
            if (model is IAmmoFiredListener listener)
            {
                await listener.OnAmmoFired(player, results);
            }
        }
    }
}