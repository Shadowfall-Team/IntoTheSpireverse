using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Shadowfall.ShadowfallCode.Ammo;

public interface IAmmoFiredListener
{
    void OnAmmoFired(Player player, IReadOnlyList<Creature> targets);
}