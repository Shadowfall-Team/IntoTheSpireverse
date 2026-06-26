using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.HoverTips;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.Colorless;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Utils;

public static class LoadAmmoHoverTip
{
    [CustomEnum] public static StaticHoverTip LoadAmmo;

    public static IEnumerable<IHoverTip> FromLoadAmmo() =>
        [HoverTipFactory.Static(LoadAmmo), ..HoverTipFactory.FromCardWithCardHoverTips<AmmoVolley>()];
}