using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.Extensions;

namespace Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

[Pool(typeof(ShadowIroncladRelicPool))]
public abstract class ShadowIroncladRelic : CustomRelicModel
{
     public override string PackedIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "ironclad", path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "ironclad", path1);
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "ironclad", "big", path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
}