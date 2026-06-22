using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Helpers;
using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace IntoTheSpireverse.Orbs;

public abstract class CustomOrbModel : OrbModel
{
    /// <summary>
    /// Override this to provide a custom sprite path for your orb.
    /// If null, uses the default sprite path based on orb ID.
    /// </summary>
    public virtual string? CustomSpritePath => null;

    /// <summary>
    /// Override this to provide a custom icon texture path for your orb.
    /// If null, uses the default icon path based on orb ID.
    /// </summary>
    public virtual string? CustomIconPath => null;

    /// <summary>
    /// Gets the sprite path - uses custom path if provided, otherwise default.
    /// </summary>
    private string SpritePath
    {
        get
        {
            if (!string.IsNullOrEmpty(this.CustomSpritePath))
                return this.CustomSpritePath;
            
            return SceneHelper.GetScenePath("orbs/orb_visuals/" + this.Id.Entry.ToLowerInvariant());
        }
    }

    /// <summary>
    /// Gets the icon path - uses custom path if provided, otherwise default.
    /// </summary>
    private string IconPath
    {
        get
        {
            if (!string.IsNullOrEmpty(this.CustomIconPath))
                return this.CustomIconPath;
            
            return ImageHelper.GetImagePath($"orbs/{this.Id.Entry.ToLowerInvariant()}.png");
        }
    }

    /// <summary>
    /// Creates the sprite node using the sprite path.
    /// </summary>
    public new Node2D CreateSprite()
    {
        Node2D sprite = PreloadManager.Cache.GetScene(this.SpritePath).Instantiate<Node2D>();
        new MegaCrit.Sts2.Core.Bindings.MegaSpine.MegaSprite((Variant)(GodotObject)sprite.GetNode((NodePath)"SpineSkeleton"))
            .GetAnimationState()
            .SetAnimation("idle_loop");
        return sprite;
    }
}