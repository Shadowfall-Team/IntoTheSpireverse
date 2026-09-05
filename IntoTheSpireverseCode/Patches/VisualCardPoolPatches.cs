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

        // check that the type is either a vanilla or IntoTheSpireverse pool? just in case anyone is doing something goofy?
        if (__result.GetType().Assembly != typeof(CardPoolModel).Assembly && __result.GetType().Assembly != typeof(MainFile).Assembly)
            return;

        // Owner throws on a canonical card rather than returning null, so it can only be asked once
        // we know the card is mutable.
        CharacterModel? owningCharModel = __instance.IsCanonical ? null : __instance.Owner?.Character;

        if (owningCharModel == null)
        {
            // Nobody owns this card, so there is no owner to ask which character is being viewed.
            // That covers both compendium cards, which are canonical, and the mutable clone
            // NInspectCardScreen makes to render an upgrade preview -- the clone is ownerless, which
            // is why upgraded reprints used to fall back to the Ironclad's frame. The Card Library's
            // selected pool filter answers it for both: browsing the Tectonic's tab should show its
            // reprints in the Tectonic's frame, upgraded or not, matching how they look in a run.
            if (CardLibraryCharacterContextPatch.ViewedCharacter is IAltCharacter and CharacterModel viewedModel
                && viewedModel.CardPool.AllCardIds.Contains(__instance.Id))
            {
                __result = viewedModel.CardPool;
            }
            return;
        }

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
