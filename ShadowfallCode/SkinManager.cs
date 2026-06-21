using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.RestSite;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode;

public static class SkinManager
{
    private readonly record struct SkinSet(string? Combat, string? RestSite, string? Merchant,
        string? CharacterSelectBg);

    private static readonly Dictionary<System.Type, SkinSet> Skins = new()
    {
        [typeof(ShadowIronclad)] = new SkinSet(
            Combat: "res://Shadowfall/images/characters/shadowironclad/ironclad_granite.png",
            RestSite: "res://Shadowfall/images/characters/shadowironclad/restsite_ironclad.png",
            Merchant: "res://Shadowfall/images/characters/shadowironclad/ironclad_shop.png",
            CharacterSelectBg: "res://Shadowfall/images/characters/shadowironclad/characterselect_granite.png"
        ),
        [typeof(ShadowRegent)] = new SkinSet(
            Combat: "res://Shadowfall/images/characters/shadowregent/regent_shadow.png",
            RestSite: "res://Shadowfall/images/characters/shadowregent/restsite_regent.png",
            Merchant: "res://Shadowfall/images/characters/shadowregent/regent_shop.png",
            CharacterSelectBg: "res://Shadowfall/images/characters/shadowregent/characterselect_regent.png"
        ),
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
        if (!Skins.TryGetValue(characterType, out var skinSet) || skinSet.Combat == null)
            return;

        SwapSpineTexture(visuals.GetCurrentBody(), visuals.SpineBody, skinSet.Combat);
    }

    public static void ApplyRestSiteSkin(NRestSiteCharacter character)
    {
        var characterType = character.Player?.Character?.GetType();
        if (characterType == null)
            return;

        if (!Skins.TryGetValue(characterType, out var skinSet) || skinSet.RestSite == null)
            return;

        foreach (var spineNode in GetSpineSpriteChildren(character))
            SwapSpineTexture(spineNode, new MegaSprite(spineNode), skinSet.RestSite);
    }

    public static void ApplyMerchantSkin(NMerchantCharacter character, System.Type characterType)
    {
        if (!Skins.TryGetValue(characterType, out var skinSet) || skinSet.Merchant == null)
            return;

        foreach (var spineNode in GetSpineSpriteChildren(character))
            SwapSpineTexture(spineNode, new MegaSprite(spineNode), skinSet.Merchant);

        character.PlayAnimation("relaxed_loop", loop: true);
    }

    public static void ApplyCharacterSelectBgSkin(Node bgContainer, System.Type characterType)
    {
        if (!Skins.TryGetValue(characterType, out var skinSet) || skinSet.CharacterSelectBg == null)
            return;

        var spineNode = FindSpineSprite(bgContainer);
        if (spineNode == null)
            return;

        SwapSpineTexture(spineNode, new MegaSprite(spineNode), skinSet.CharacterSelectBg);
    }

    private static Node2D? FindSpineSprite(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is Node2D node2D && node2D.GetClass() == "SpineSprite")
                return node2D;

            var nested = FindSpineSprite(child);
            if (nested != null)
                return nested;
        }

        return null;
    }

    private static IEnumerable<Node2D> GetSpineSpriteChildren(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is Node2D node2D && node2D.GetClass() == "SpineSprite")
                yield return node2D;
        }
    }

    private static void SwapSpineTexture(
        GodotObject spineNode,
        MegaSprite spineBody,
        string replacementTexturePath
    )
    {
        var skeletonData = spineNode.Get("skeleton_data_res").AsGodotObject();
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

        var texture = ResourceLoader.Load<Texture2D>(replacementTexturePath);
        if (texture == null)
            return;

        var textureCopy = (Texture2D)texture.Duplicate();
        textureCopy.TakeOverPath(originalTexturePath);

        var freshSkeletonData = ResourceLoader.Load(
            skeletonDataPath,
            cacheMode: ResourceLoader.CacheMode.IgnoreDeep
        );

        spineBody.SetSkeletonDataRes(
            new MegaSkeletonDataResource(Variant.From(freshSkeletonData))
        );
    }
}
