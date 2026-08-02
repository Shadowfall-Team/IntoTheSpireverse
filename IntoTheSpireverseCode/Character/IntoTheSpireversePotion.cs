using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public abstract class IntoTheSpireversePotion : CustomPotionModel
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