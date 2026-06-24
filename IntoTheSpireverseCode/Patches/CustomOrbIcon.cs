using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using Godot;
using MegaCrit.Sts2.Core.Assets;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(OrbModel), "Icon", MethodType.Getter)]
internal class CustomOrbIcon
{
    [HarmonyPrefix]
    private static bool UseCustomIcon(OrbModel __instance, ref CompressedTexture2D __result)
    {
        if (!(__instance is IntoTheSpireverse.Orbs.CustomOrbModel customOrbModel))
            return true;

        if (string.IsNullOrEmpty(customOrbModel.CustomIconPath))
            return true;

        __result = PreloadManager.Cache.GetCompressedTexture2D(customOrbModel.CustomIconPath);
        return false;
    }
}