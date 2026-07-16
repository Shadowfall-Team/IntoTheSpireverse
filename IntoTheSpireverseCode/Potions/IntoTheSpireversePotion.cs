using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Potions;

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

[Pool(typeof(ShadowRegentPotionPool))]
public abstract class ShadowRegentPotion : IntoTheSpireversePotion;