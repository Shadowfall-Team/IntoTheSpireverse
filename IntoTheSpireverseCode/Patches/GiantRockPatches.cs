using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.CardTags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(CardModel), "CanonicalTags", MethodType.Getter)]
public static class GiantRockPatches
{
    public static void Postfix(CardModel __instance, ref HashSet<CardTag> __result)
    {
        if (__instance is GiantRock)
            __result.Add(IntoTheSpireverseCardTags.Rock);
    }
}