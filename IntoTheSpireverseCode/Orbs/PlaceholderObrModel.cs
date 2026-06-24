using MegaCrit.Sts2.Core.Helpers;

namespace IntoTheSpireverse.Orbs;

public abstract class PlaceholderOrbModel : CustomOrbModel
{
    /// <summary>
    /// Override this to specify which orb ID to use as a placeholder.
    /// Default is "lightning" (the Lightning Orb).
    /// </summary>
    public virtual string PlaceholderID => "lightning_orb";

    /// <summary>
    /// Gets the custom sprite path using the placeholder ID.
    /// </summary>
    public override string? CustomSpritePath
    {
        get => "orbs/orb_visuals/" + this.PlaceholderID;
    }

    /// <summary>
    /// Gets the custom icon path using the placeholder ID.
    /// </summary>
    public override string? CustomIconPath
    {
        get => ImageHelper.GetImagePath($"orbs/{this.PlaceholderID}.png");
    }
}