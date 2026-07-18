using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Ammo;

public static class LoadAmmoHoverTip
{
    public static IEnumerable<IHoverTip> FromLoadAmmo(Player? player = null)
    {
        var damage = player != null ? AmmoResource.GetShotDamage(player) : AmmoResource.BaseDamage;
        return [AmmoTooltipWithDamage(damage, "LOAD_AMMO")];
    }

    public static IEnumerable<IHoverTip> ForFireButton(Player? player = null)
    {
        var damage = player != null ? AmmoResource.GetShotDamage(player) : AmmoResource.BaseDamage;
        return [AmmoTooltipWithDamage(damage, "AMMO_BUTTON")];
    }

    private static HoverTip AmmoTooltipWithDamage(decimal damage, string entry)
    {
        var title = new LocString("static_hover_tips", $"INTOTHESPIREVERSE-{entry}.title");
        var description = new LocString("static_hover_tips", $"INTOTHESPIREVERSE-{entry}.description");
        description.Add("Damage", damage);
        return new HoverTip(title, description);
    }
}