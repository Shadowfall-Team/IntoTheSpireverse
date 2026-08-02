using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;

[Pool(typeof(ShadowSilentRelicPool))]
public abstract class ShadowSilentRelic : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "silent", path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "silent", path1);
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "silent", "big", path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
}