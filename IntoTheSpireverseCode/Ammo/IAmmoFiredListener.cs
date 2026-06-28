using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

/// <summary>Pre-damage hook. FreeShot decrements itself here.</summary>
public interface IAmmoFiringListener
{
    Task OnAmmoFiring(Player player);
}


/// <summary>Post-damage hook.</summary>
public interface IAmmoFiredListener
{
    Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results);
}

public interface IAmmoLoadedListener
{
    Task OnAmmoLoaded();
}

public interface IModifiesShotCost
{
    int ModifyShotCost(int current);
}