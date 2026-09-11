using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(CardPileCmd))]
public static class CardPileCmdPatch
{
    public static Dictionary<Type, PileType> _cardsToModifyDiscarding = new() { {typeof(Disguise), PileType.Draw} };

    [HarmonyPatch(nameof(CardPileCmd.Add), [typeof(CardModel), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool)])]
    [HarmonyPrefix]
    public static void DiscardRedirectionPatch(CardModel card, ref CardPile newPile)
    {
        if (newPile.Type is PileType.Discard && _cardsToModifyDiscarding.ContainsKey(card.GetType()))
        {
            newPile = _cardsToModifyDiscarding[card.GetType()].GetPile(card.Owner);
        }
    }
}


