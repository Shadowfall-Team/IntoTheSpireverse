using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Shadowfall.ShadowfallCode.Extensions;

namespace Shadowfall.ShadowfallCode.Powers;

public abstract class ShadowPowerModel : CustomPowerModel
{
    //Loads from Shadowfall/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            if (ResourceLoader.Exists(path))
            {
                return path;
            }
            else
            {
                MainFile.Logger.Warn($"Couldn't find packed icon at {path}");
                return "power.png".PowerImagePath();
            }
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}