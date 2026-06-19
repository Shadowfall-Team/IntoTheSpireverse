using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Powers;

public abstract class ShadowPowerModel : CustomPowerModel
{
    //Loads from IntoTheSpireverse/images/powers/your_power.png
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