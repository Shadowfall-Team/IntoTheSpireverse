using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.RestSite;

namespace Shadowfall.ShadowfallCode;

[HarmonyPatch(typeof(NRestSiteCharacter), "_Ready")]
public static class ApplyRestSiteSkinPatch
{
    public static void Prefix(NRestSiteCharacter __instance)
    {
        try
        {
            SkinManager.ApplyRestSiteSkin(__instance);
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Failed to apply rest site skin: {e}");
        }
    }
}
