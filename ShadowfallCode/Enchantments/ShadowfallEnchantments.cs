#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Shadowfall.ShadowfallCode.Extensions;

#endregion

namespace Shadowfall.ShadowfallCode.Enchantments;

public class ShadowfallEnchantments : CustomEnchantmentModel
{
    protected override string? CustomIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EnchantmentImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }
}