using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace IntoTheSpireverse.IntoTheSpireverseCode;

// the baselib custom enum for this seems excessive
// and there's that one uncommon bug where custom enums don't work right?
public enum IntoTheSpireverseHoverTips
{
    None,
    Cargo,
    Muddle,
    Indirectly,
    Modify,
    Pickup,
}

public static class HoverTipExtensions
{
    extension(HoverTipFactory)
    {
        public static IHoverTip Static(IntoTheSpireverseHoverTips tip, params DynamicVar[] vars)
        {
            string text = StringHelper.Slugify(tip.ToString());
            string prefix = nameof(IntoTheSpireverse).ToUpperInvariant();

            var title = L10NStatic(prefix + text + ".title");
            var description = L10NStatic(prefix + text + ".description");
            foreach (DynamicVar dynamicVar in vars)
            {
                title.Add(dynamicVar);
                description.Add(dynamicVar);
            }
            return new HoverTip(title, description);
        }
    }

    private static LocString L10NStatic(string entry)
    {
        return new ("static_hover_tips", entry);
    }
}
