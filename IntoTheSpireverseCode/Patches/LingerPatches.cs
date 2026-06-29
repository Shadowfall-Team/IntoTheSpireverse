using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HasTurnEndInHandEffect), MethodType.Getter)]
public class LingerHasTurnEndPatch
{
    [HarmonyPostfix]
    static void Postfix(CardModel __instance, ref bool __result)
    {
        if (__instance.Keywords.Contains(IntoTheSpireverseKeywords.Linger))
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
        if (!card.Keywords.Contains(IntoTheSpireverseKeywords.Linger)) return;

        // Card opted into draw pile redirect (beats both discard and Ethereal exhaust)
        if ((newPile.Type == PileType.Discard || newPile.Type == PileType.Exhaust)
            && LingerHelper.PendingLingerRedirect.Remove(card))
        {
            newPile = PileType.Draw.GetPile(card.Owner);
            position = CardPilePosition.Random;
            return;
        }

        // Card should stay in hand (Retain, Runic Pyramid, etc.) — only for discard
        // Note that Linger cards with Retain jump to the rightmost Hand position after their effect triggers. Possible issue?
        if (newPile.Type == PileType.Discard
            && (card.ShouldRetainThisTurn || !Hook.ShouldFlush(card.Owner.Creature.CombatState, card.Owner)))
        {
            newPile = PileType.Hand.GetPile(card.Owner);
            position = CardPilePosition.Bottom;
        }
    }
}

// Another patch on CombatManager's DoTurnEnd could be added to reduce boilerplate in each card class which implements Linger,
// but it's async and it'd need to be a transpiler, so...

public static class LingerHelper
{
    internal static readonly HashSet<CardModel> PendingLingerRedirect = new();
    public static event Func<CardModel, PlayerChoiceContext, Task>? OnLingerTriggered;
    public static int GetTriggerCount(CardModel card)
    {
        return 1;
    }

    public static async Task NotifyLingerTriggered(CardModel card, PlayerChoiceContext choiceContext)
    {
        if (OnLingerTriggered != null)
        {
            foreach (var handler in OnLingerTriggered.GetInvocationList().Cast<Func<CardModel, PlayerChoiceContext, Task>>())
            {
                await handler(card, choiceContext);
            }
        }
    }
}