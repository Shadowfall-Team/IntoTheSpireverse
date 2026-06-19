using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Extensions;

namespace Shadowfall.ShadowfallCode.Potions;

[Pool(typeof(ShadowDefectPotionPool))]
public abstract class ShadowfallPotion : CustomPotionModel
{
    public override string? CustomPackedImagePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
            return ResourceLoader.Exists(path) ? path : null;
        }
    }
}

[Pool(typeof(ShadowRegentPotionPool))]
public abstract class ShadowRegentPotion : ShadowfallPotion;