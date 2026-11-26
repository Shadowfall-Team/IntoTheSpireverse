using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(OrbModel), "SpritePath", MethodType.Getter)]
internal static class CustomOrbSpritePath
{
    [HarmonyPrefix]
    internal static bool UseCustomSpritePath(OrbModel __instance, ref string __result)
    {
        if (__instance is not CustomOrbModel customOrbModel)
            return true;

        if (string.IsNullOrEmpty(customOrbModel.CustomSpritePath))
            return true;

        __result = customOrbModel.CustomSpritePath;
        return false;
    }
}
