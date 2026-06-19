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
        string cardId = __instance.Id.ToString();

        // 1. Check if the user has a custom path saved
        var userHsv = CardArtRoller.GetCardData(cardId);
        if (userHsv != null && !string.IsNullOrWhiteSpace(userHsv.PortraitPath))
        {
            __result = userHsv.PortraitPath;
            return false; // Skip the vanilla getter entirely
        }

        // 2. Check if the modder default has a custom path saved
        var defaultHsv = CardArtRoller.GetDefaultHsvForCard(cardId);
        if (defaultHsv != null && !string.IsNullOrWhiteSpace(defaultHsv.PortraitPath))
        {
            __result = defaultHsv.PortraitPath;
            return false; // Skip the vanilla getter entirely
        }

        // 3. Otherwise, let the vanilla game run its normal path logic
        return true; 
    }
}