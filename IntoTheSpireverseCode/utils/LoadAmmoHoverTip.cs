using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.HoverTips;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

namespace IntoTheSpireverse.IntoTheSpireverseCode.utils;

public static class LoadAmmoHoverTip
{
    [CustomEnum] public static StaticHoverTip LoadAmmo;

    public static IEnumerable<IHoverTip> FromLoadAmmo()
    {
        var list = new List<IHoverTip> { HoverTipFactory.Static(LoadAmmo) };
        list.AddRange(HoverTipFactory.FromCardWithCardHoverTips<AmmoVolley>());
        return new List<IHoverTip>(list);
    }
}