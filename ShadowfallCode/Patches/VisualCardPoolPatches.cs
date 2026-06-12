using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Patches;

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

        // check that the type is either a vanilla or shadowfall pool? just in case anyone is doing something goofy?
        if (__result.GetType().Assembly != typeof(CardPoolModel).Assembly && __result.GetType().Assembly != typeof(MainFile).Assembly)
            return;

        CharacterModel? owningCharModel = __instance.Owner?.Character;
        if (owningCharModel == null) { return; }

        if (owningCharModel is IAltCharacter ||
            ModelDb.AllCharacters.Any(a => a is IAltCharacter ac && ac.BaseCharacterModel == owningCharModel))
        {
            // yoinked from Arquebus (ty)
            CharacterModel? _mirrorCharacterModel = owningCharModel is IAltCharacter ownerAltCharacter
                ? ownerAltCharacter.BaseCharacterModel
                : __instance.Owner?.RunState.Rng.CombatCardSelection.NextItem(ModelDb.AllCharacters
                        .Where(c => c is IAltCharacter ac && ac.BaseCharacterModel == owningCharModel));

            // if the card pool is the mirror character's one, swap the pool to the active character's pool
            // Potentially look into storing which character the card was obtained from, and only changing it based on that character
            if (__result == _mirrorCharacterModel?.CardPool)
            {
                __result = owningCharModel.CardPool;
            }
        }
    }
}
