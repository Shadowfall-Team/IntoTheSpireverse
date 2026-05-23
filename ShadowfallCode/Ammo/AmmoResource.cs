using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Shadowfall.ShadowfallCode.Ammo;

public static class AmmoResource
{
    public static readonly SpireField<PlayerCombatState, AmmoState?> State = new(() => null);

    //(state, oldAmmo, newAmmo)
    public static event Action<PlayerCombatState, int, int>? AmmoChanged;

    public static event Action<Player, IReadOnlyList<Creature>>? OnAmmoFired;

    public static AmmoState GetOrCreateState(Player player)
    {
        var combatState = player.PlayerCombatState;
        var state = State[combatState];
        if (state == null)
        {
            state = new AmmoState(player);
            State[combatState] = state;
        }
        return state;
    }

    public static int GetAmmo(Player player) => GetOrCreateState(player).Ammo;

    public static void GainAmmo(int amount, Player player)
    {
        var state = GetOrCreateState(player);
        var old = state.Ammo;
        state.Ammo += amount;
        AmmoChanged?.Invoke(player.PlayerCombatState, old, state.Ammo);
    }

    public static void LoseAmmo(int amount, Player player)
    {
        var state = GetOrCreateState(player);
        var old = state.Ammo;
        state.Ammo = Math.Max(0, state.Ammo - amount);
        AmmoChanged?.Invoke(player.PlayerCombatState, old, state.Ammo);
    }

    public static void SetAmmo(int amount, Player player)
    {
        var state = GetOrCreateState(player);
        var old = state.Ammo;
        state.Ammo = amount;
        if (old != state.Ammo)
            AmmoChanged?.Invoke(player.PlayerCombatState, old, state.Ammo);
    }

    public static void InvokeOnAmmoFired(Player player, IReadOnlyList<Creature> targets)
    {
        OnAmmoFired?.Invoke(player, targets);
    }
}
