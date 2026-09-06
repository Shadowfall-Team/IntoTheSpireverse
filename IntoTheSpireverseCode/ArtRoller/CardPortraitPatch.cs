using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(CardModel))]
public class CardModelPortraitPatch
{
    [HarmonyPatch(nameof(CardModel.PortraitPath), MethodType.Getter)]
    [HarmonyPrefix]
    static bool OverridePortraitPath(CardModel __instance, ref string __result)
    {
        // Resolve honours alt-character scoping, so a base game card reprinted into an alt
        // character's pool can carry its own portrait without altering the original character's.
        var hsv = CardArtRoller.Resolve(__instance);
        if (hsv != null && !string.IsNullOrWhiteSpace(hsv.PortraitPath))
        {
            __result = hsv.PortraitPath;
            return false; // Skip the vanilla getter entirely
        }

        // Otherwise, let the vanilla game run its normal path logic
        return true;
    }
}