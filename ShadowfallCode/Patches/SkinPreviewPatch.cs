using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace Shadowfall.ShadowfallCode;

[HarmonyPatch]
public static class SkinPreviewPatch
{
    const float PreviewOffsetX = 500f;

    static NCreatureVisuals? _previewVisuals;
    static Node2D? _previewContainer;
    static CharacterModel? _previewCharModel;

    static void RefreshPreview()
    {
        if (_previewVisuals != null && GodotObject.IsInstanceValid(_previewVisuals))
        {
            _previewVisuals.QueueFree();
            _previewVisuals = null;
        }

        if (_previewContainer == null || !GodotObject.IsInstanceValid(_previewContainer)) return;
        if (_previewCharModel == null) return;

        _previewVisuals = _previewCharModel.CreateVisuals();
        _previewContainer.AddChild(_previewVisuals);

        try
        {
            SkinManager.ApplyTextureToVisuals(_previewVisuals, _previewCharModel.GetType());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error(e.ToString());
        }

        _previewVisuals.Position = new Vector2(PreviewOffsetX, _previewVisuals.Bounds.Size.Y * 0.5f);

        if (_previewVisuals.HasSpineAnimation && _previewVisuals.SpineBody != null)
            _previewVisuals.SpineBody.GetAnimationState().SetAnimation("idle_loop");
    }

    [HarmonyPatch(typeof(NCharacterSelectScreen), "_Ready")]
    [HarmonyPostfix]
    static void CharSelectScreenReady(NCharacterSelectScreen __instance)
    {
        _previewCharModel = null;
        _previewVisuals = null;  
        _previewContainer = new Node2D();
        _previewContainer.Name = "Shadowfall_Preview";
        _previewContainer.Position = new Vector2(960, 650);
        __instance.AddChild(_previewContainer);
        var bgNode = __instance.GetNode("AnimatedBg");
        __instance.MoveChild(_previewContainer, bgNode.GetIndex() + 1);
    }

    [HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
    [HarmonyPostfix]
    static void OnSelectCharacter(NCharacterSelectScreen __instance, CharacterModel characterModel, NCharacterSelectButton charSelectButton)
    {
        _previewCharModel = (!charSelectButton.IsLocked && !charSelectButton.IsRandom)
            ? characterModel : null;
        RefreshPreview();

        if (charSelectButton.IsLocked || charSelectButton.IsRandom)
            return;

        try
        {
            var bgContainer = __instance.GetNode("AnimatedBg");
            SkinManager.ApplyCharacterSelectBgSkin(bgContainer, characterModel.GetType());
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Failed to apply character select bg skin: {e}");
        }
    }
}