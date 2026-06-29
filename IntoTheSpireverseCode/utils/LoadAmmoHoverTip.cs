using BaseLib.Patches.Content;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils;

public static class LoadAmmoHoverTip
{
    [CustomEnum] public static StaticHoverTip LoadAmmo;

    public static IEnumerable<IHoverTip> FromLoadAmmo(Player? player = null)
    {
        var damage = player != null ? AmmoResource.GetShotDamage(player) : AmmoResource.BaseDamage;
        return [FromAmmoButton(damage)];
    }

    private static IHoverTip FromAmmoButton(decimal damage)
    {
        var title = new LocString("static_hover_tips", "INTOTHESPIREVERSE-AMMO_BUTTON.title");
        var description = new LocString("static_hover_tips", "INTOTHESPIREVERSE-AMMO_BUTTON.description");
        description.Add("Damage", damage);
        return new HoverTip(title, description, null);
    }
}