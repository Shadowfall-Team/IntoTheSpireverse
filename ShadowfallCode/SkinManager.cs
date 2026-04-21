using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Shadowfall.ShadowfallCode;

public static class SkinManager
{
    private const string SkeletonPath = "res://animations/characters/defect/defect_skel_data.tres";
    private const string ReplacementTexturePath = "res://skins/shadowdefect.png";

    private static string? _originalTexturePath;
    private static Texture2D? _originalTexture;

    public static void ApplyTextureToVisuals(NCreatureVisuals visuals)
    {
        var body = visuals.GetCurrentBody();
        var skeletonData = body.Get("skeleton_data_res").AsGodotObject();
        var skeletonDataPath = ((Resource)skeletonData).ResourcePath;

        if (skeletonDataPath != SkeletonPath)
            return;

        var atlasRes = skeletonData.Get("atlas_res").AsGodotObject();
        var textures = atlasRes.Get("textures").AsGodotArray();
        var textureResource = (Texture2D)textures[0].AsGodotObject();
        var currentTexturePath = textureResource.ResourcePath;

        if (_originalTexturePath == null)
        {
            if (string.IsNullOrEmpty(currentTexturePath))
                return;

            _originalTexturePath = currentTexturePath;
            _originalTexture = (Texture2D)textureResource.Duplicate();
        }

        var texture = ResourceLoader.Load<Texture2D>(ReplacementTexturePath);
        if (texture == null)
            return;

        var textureCopy = (Texture2D)texture.Duplicate();
        textureCopy.TakeOverPath(_originalTexturePath);

        var freshSkeletonData = ResourceLoader.Load(
            SkeletonPath,
            cacheMode: ResourceLoader.CacheMode.IgnoreDeep
        );

        visuals.SpineBody.SetSkeletonDataRes(
            new MegaSkeletonDataResource(Variant.From(freshSkeletonData))
        );
    }
}