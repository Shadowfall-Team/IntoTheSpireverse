using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using Godot;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(OrbModel), "SpritePath", MethodType.Getter)]
internal class CustomOrbSpritePath
{
    [HarmonyPrefix]
    private static bool UseCustomSpritePath(OrbModel __instance, ref string __result)
    {
        if (!(__instance is IntoTheSpireverse.Orbs.CustomOrbModel customOrbModel))
            return true;

        if (string.IsNullOrEmpty(customOrbModel.CustomSpritePath))
            return true;

        __result = customOrbModel.CustomSpritePath;
        return false;
    }
}