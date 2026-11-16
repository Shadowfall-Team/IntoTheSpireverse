using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(OrbModel), "IconPath", MethodType.Getter)]
internal class CustomOrbIconPath
{
    [HarmonyPrefix]
    private static bool UseCustomIconPath(OrbModel __instance, ref string __result)
    {
        if (!(__instance is CustomOrbModel customOrbModel))
            return true;

        if (string.IsNullOrEmpty(customOrbModel.CustomIconPath))
            return true;

        __result = customOrbModel.CustomIconPath;
        return false;
    }
}
