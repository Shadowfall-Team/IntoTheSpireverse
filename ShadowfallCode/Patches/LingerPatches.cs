using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HasTurnEndInHandEffect), MethodType.Getter)]
public class LingerHasTurnEndPatch
{
    [HarmonyPostfix]
    static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance.Keywords.Contains(ShadowfallKeywords.Linger))
        {
            if (!__result)
                __result = true;
        }
    }
}

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add),
    new[] { typeof(CardModel), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool) })]
public class LingerDiscardRedirectPatch
{
    [HarmonyPrefix]
    static void Prefix(CardModel card, ref CardPile newPile, ref CardPilePosition position)
    {
        if (!card.Keywords.Contains(ShadowfallKeywords.Linger)) return;

        // Card opted into draw pile redirect (beats both discard and Ethereal exhaust)
        if ((newPile.Type == PileType.Discard || newPile.Type == PileType.Exhaust)
            && LingerHelper.PendingLingerRedirect.Remove(card))
        {
            newPile = PileType.Draw.GetPile(card.Owner);
            position = CardPilePosition.Random;
            return;
        }

        // Card should stay in hand (Retain, Runic Pyramid, etc.) — only for discard
        if (newPile.Type == PileType.Discard
            && (card.ShouldRetainThisTurn || !Hook.ShouldFlush(card.Owner.Creature.CombatState, card.Owner)))
        {
            newPile = PileType.Hand.GetPile(card.Owner);
            position = CardPilePosition.Bottom;
        }
    }
}

public static class LingerHelper
{
    internal static readonly HashSet<CardModel> PendingLingerRedirect = new();
}