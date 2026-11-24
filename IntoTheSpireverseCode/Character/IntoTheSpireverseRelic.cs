using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public abstract class IntoTheSpireverseRelic(string subPath) : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", subPath, path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", subPath, path1);
            return ResourceLoader.Exists(path) ? path : "relic_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path1 = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png";
            var path = Path.Join(MainFile.ModId, "images", "relics", subPath, "big", path1);
            return ResourceLoader.Exists(path) ? path : "relic.png".BigRelicImagePath();
        }
    }
}
