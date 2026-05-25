using MegaCrit.Sts2.Core.Entities.Players;
using Shadowfall.ShadowfallCode.Cards.Colorless;

namespace Shadowfall.ShadowfallCode.Ammo;

public class AmmoState
{
    public int Ammo { get; set; }
    public AmmoVolley PhantomCard { get; }

    public AmmoState(Player player)
    {
        Ammo = 0;
        PhantomCard = player.Creature.CombatState.CreateCard<AmmoVolley>(player);
    }
}
