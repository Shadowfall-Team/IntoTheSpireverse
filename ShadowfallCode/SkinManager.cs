using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode;

public static class SkinManager
{
    private static readonly Dictionary<System.Type, string> Skins = new()
    {
        [typeof(ShadowIronclad)] = "res://Shadowfall/images/characters/shadowironclad/ironclad_granite.png",
        [typeof(ShadowRegent)] = "res://Shadowfall/images/characters/shadowregent/regent_shadow.png",
    };

    private static readonly Dictionary<string, string> _originalTexturePaths = new();

    public static void ApplyTextureToVisuals(NCreatureVisuals visuals)
    {
        if (visuals.GetParent() is not NCreature nCreature)
            return;

        var character = nCreature.Entity?.Player?.Character;
        if (character == null)
            return;

        ApplyTextureToVisuals(visuals, character.GetType());
    }

    public static void ApplyTextureToVisuals(NCreatureVisuals visuals, System.Type characterType)
    {
        if (!Skins.TryGetValue(characterType, out var ReplacementTexturePath))
            return;

        var body = visuals.GetCurrentBody();
        var skeletonData = body.Get("skeleton_data_res").AsGodotObject();
        var skeletonDataPath = ((Resource)skeletonData).ResourcePath;

        var atlasRes = skeletonData.Get("atlas_res").AsGodotObject();
        var textures = atlasRes.Get("textures").AsGodotArray();
        var textureResource = (Texture2D)textures[0].AsGodotObject();
        var currentTexturePath = textureResource.ResourcePath;

        if (!_originalTexturePaths.TryGetValue(skeletonDataPath, out var originalTexturePath))
        {
            if (string.IsNullOrEmpty(currentTexturePath))
                return;

            originalTexturePath = currentTexturePath;
            _originalTexturePaths[skeletonDataPath] = originalTexturePath;
        }

        var texture = ResourceLoader.Load<Texture2D>(ReplacementTexturePath);
        if (texture == null)
            return;

        var textureCopy = (Texture2D)texture.Duplicate();
        textureCopy.TakeOverPath(originalTexturePath);

        var freshSkeletonData = ResourceLoader.Load(
            skeletonDataPath,
            cacheMode: ResourceLoader.CacheMode.IgnoreDeep
        );

        visuals.SpineBody.SetSkeletonDataRes(
            new MegaSkeletonDataResource(Variant.From(freshSkeletonData))
        );
    }
}