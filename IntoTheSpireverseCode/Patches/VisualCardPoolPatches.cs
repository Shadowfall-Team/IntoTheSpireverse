using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch]
public static class VisualCardPoolPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.VisualCardPool), MethodType.Getter)]
    public static void get_VisualCardPool_Postfix(CardModel __instance, ref CardPoolModel __result)
    {

        // Currently not attempting to fix the compendium
        // There doesn't appear to be a good way of getting the current filter being applied,
        // or the character the filter belongs to? 
        if (__instance.IsCanonical) { return; }

        // check that the type is either a vanilla or IntoTheSpireverse pool? just in case anyone is doing something goofy?
        if (__result.GetType().Assembly != typeof(CardPoolModel).Assembly && __result.GetType().Assembly != typeof(MainFile).Assembly)
            return;

        CharacterModel? owningCharModel = __instance.Owner?.Character;
        if (owningCharModel == null) { return; }

        // ref params can't be captured in lambdas
        var currentPool = __result;

        if (owningCharModel is IAltCharacter ownerAltCharacter)
        {
            // Alt character: if the card is displaying the base character's pool, swap to ours
            if (currentPool == ownerAltCharacter.BaseCharacterModel.CardPool && owningCharModel.CardPool.AllCardIds.Contains(__instance.Id))
            {
                __result = owningCharModel.CardPool;
            }
        }
        else if (ModelDb.AllCharacters.Any(c =>
                     c is IAltCharacter ac &&
                     ac.BaseCharacterModel == owningCharModel &&
                     currentPool == c.CardPool &&
                     owningCharModel.CardPool.AllCardIds.Contains(__instance.Id)))
        {
            // Base character: if the card is displaying any alt character's pool (e.g. cards
            // gained via MirrorMirror), swap to the owner's pool instead.
            __result = owningCharModel.CardPool;
        }
    }
}
