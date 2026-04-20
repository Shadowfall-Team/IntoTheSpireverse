using System.Diagnostics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using Shadowfall.ShadowfallCode.Character;
using Shadowfall.ShadowfallCode.ui;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(NCharacterSelectScreen))]
public class NCharacterSelectScreenPatches
{
    private const float yOffset = 40;

    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    public static void ReadyPostfix(NCharacterSelectScreen __instance)
    {
        if (!ModelDb.AllCharacters.Any(c => c is IAltCharacter)) return;

        __instance._ascensionPanel.Position = new Vector2(__instance._ascensionPanel.Position.X,
            __instance._ascensionPanel.Position.Y - yOffset);
        __instance._infoPanel.Position = new Vector2(__instance._infoPanel.Position.X,
            __instance._infoPanel.Position.Y - yOffset);
    }
}

[HarmonyPatch(typeof(NCharacterSelectButton))]
public class NCharacterSelectButtonPatches
{
    private const string _scenePath = "res://" + MainFile.ModId + "/scenes/CharAltArrow.tscn";
    private const string _shaderMaterialPath = "res://materials/vfx/hsv.tres";

    private static Material hsv = ResourceLoader.Load<Material>(_shaderMaterialPath);

    [HarmonyPatch("Init")]
    [HarmonyPrefix]
    public static void InitPostfix(NCharacterSelectButton __instance,
        CharacterModel character, ICharacterSelectButtonDelegate del)
    {
        // MainFile.Logger.Info("NCharacterSelectButtonPatches::InitPostfix");
        // var stacktrace = new StackTrace();
        // MainFile.Logger.Info(stacktrace.ToString());
        var altCharacterCount = ModelDb.AllCharacters.Count(c =>
            c is IAltCharacter altCharacter && altCharacter.BaseCharacterModel == character);
        if (altCharacterCount <= 0) return;

        var arrowButton = ResourceLoader.Load<PackedScene>(_scenePath).Instantiate<NCharAltArrow>();
        var arrowTextureRect = arrowButton.GetNode<TextureRect>("TextureRect");
        arrowTextureRect.Material = hsv.Duplicate() as Material;

        //6 + (portrait width/2) - width of arrow
        arrowButton.Position = new Vector2(50 - arrowButton.Size.X / 2, -(arrowButton.Size.Y / 2) - 15);
        arrowButton.ClickDelegate = del;

        arrowButton.Characters = ModelDb.AllCharacters
            .Where(c => c is IAltCharacter altCharacter && altCharacter.BaseCharacterModel == character)
            .ToList();
        arrowButton.Characters.Add(character);

        __instance.AddChild(arrowButton);
    }
}