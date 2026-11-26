using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(OrbModel), "IconPath", MethodType.Getter)]
internal static class CustomOrbIconPath
{
    [HarmonyPrefix]
    internal static bool UseCustomIconPath(OrbModel __instance, ref string __result)
    {
        if (__instance is not CustomOrbModel customOrbModel)
            return true;

        if (string.IsNullOrEmpty(customOrbModel.CustomIconPath))
            return true;

        __result = customOrbModel.CustomIconPath;
        return false;
    }
}
