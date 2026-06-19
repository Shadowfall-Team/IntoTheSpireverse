using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace IntoTheSpireverse.IntoTheSpireverseCode;

[HarmonyPatch(typeof(NCreatureVisuals), "_Ready")]
public static class ApplySkinPatch
{
    public static void Postfix(NCreatureVisuals __instance)
    {
        try
        {
            SkinManager.ApplyTextureToVisuals(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to apply skin: {e}");
        }
    }
}