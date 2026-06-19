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



    [HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
    [HarmonyPostfix]
    static void OnSelectCharacter(NCharacterSelectScreen __instance, CharacterModel characterModel, NCharacterSelectButton charSelectButton)
    {

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