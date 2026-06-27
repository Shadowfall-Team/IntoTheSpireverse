using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public interface IAmmoFiredListener
{
    Task OnAmmoFired(Player player, IEnumerable<List<DamageResult>> results);
}

public interface IAmmoLoadedListener
{
    Task OnAmmoLoaded();
}