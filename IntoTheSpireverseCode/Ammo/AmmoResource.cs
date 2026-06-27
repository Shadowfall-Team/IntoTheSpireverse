using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public static class AmmoResource
{
    //TODO: Exposed for muzzle flash on ship for later
    // public static event Action<Player, CardPlay>? OnAmmoFired;

    public static int GetAmmo(Player player) => AmmoCardPile.AmmoPileType.GetPile(player).Cards.Count;

    public static async Task GainAmmo(int amount, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;

        for (var i = 0; i < amount; i++)
        {
            var card = combatState.CreateCard<AmmoVolley>(player);
            await CardPileCmd.AddGeneratedCardToCombat(card, AmmoCardPile.AmmoPileType, player);
            
            foreach (var model in player.Creature.CombatState.IterateHookListeners().ToList())
            {
                if (model is IAmmoLoadedListener listener)
                    await listener.OnAmmoLoaded();
            }
        }
    }

    public static async Task InvokeOnAmmoFired(Player player, IEnumerable<List<DamageResult>> results)
    {
        // OnAmmoFired?.Invoke(player, shotFired);

        foreach (var model in player.Creature.CombatState.IterateHookListeners().ToList())
        {
            if (model is IAmmoFiredListener listener)
                await listener.OnAmmoFired(player, results);
        }
    }
}
