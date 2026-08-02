#region

using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

#endregion

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class IntoTheSpireverseEnchantment : CustomEnchantmentModel
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