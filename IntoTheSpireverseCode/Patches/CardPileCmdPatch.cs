using BaseLib.Abstracts;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch]
public static class CardPileCmdPatch
{
    public static Dictionary<Type, Tuple<PileType, CardPilePosition>> _cardsToModifyDiscarding =
        new() { {typeof(Disguise), new Tuple<PileType, CardPilePosition> (PileType.Draw, CardPilePosition.Top)} };

    internal static readonly HashSet<CardModel> PendingRedirect = new();
    public static bool insideDiscard;

    [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add), [typeof(CardModel), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool)])]
    [HarmonyPrefix]
    public static void DiscardRedirectionPatch(CardModel card, ref CardPile newPile, ref CardPilePosition position)
    {
        var type = card.GetType();
        if (newPile.Type is PileType.Discard && _cardsToModifyDiscarding.ContainsKey(type) && PendingRedirect.Contains(card))
        {
            newPile = _cardsToModifyDiscarding[type].Item1.GetPile(card.Owner);
            position = _cardsToModifyDiscarding[type].Item2;
            PendingRedirect.Remove(card);
        }
    }

    [HarmonyPatch(typeof(CardCmd), nameof(CardCmd.DiscardAndDraw))]
    [HarmonyPrefix]
    public static void CheckForCardShouldDiscard(IEnumerable<CardModel> cardsToDiscard)
    {
        foreach (var card in cardsToDiscard)
        {
            if (!card.IsSlyThisTurn)
            {
                PendingRedirect.Add(card);
            }
        }
    }
}

