using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

public class VolleyDamagePower : ShadowPowerModel, IModifiesAmmoShotDamage
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public decimal ModifyAmmoShotDamage(Player player, decimal current)
        => player.Creature == Owner ? current + Amount : current;
}
