using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.HoverTips;
using Shadowfall.ShadowfallCode.Cards.Colorless;

namespace Shadowfall.ShadowfallCode.utils;

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