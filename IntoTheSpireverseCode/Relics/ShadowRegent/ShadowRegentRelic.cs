using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;

[Pool(typeof(ShadowRegentRelicPool))]
public abstract class ShadowRegentRelic : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "regent", path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "regent", path1);
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", "regent", "big", path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
}