using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Commands;

public static class LoadAmmoCmd
{
    public static async Task LoadAmmo(decimal amount, Player player, AbstractModel? source)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return;

        await AmmoResource.GainAmmo((int)amount, player);
    }
}
