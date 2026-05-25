using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Ammo;

namespace Shadowfall.ShadowfallCode.Commands;

public static class LoadAmmoCmd
{
    public static Task LoadAmmo(decimal amount, Player player, AbstractModel? source)
    {
        if (CombatManager.Instance.IsOverOrEnding)
            return Task.CompletedTask;

        AmmoResource.GainAmmo((int)amount, player);
        return Task.CompletedTask;
    }
}
