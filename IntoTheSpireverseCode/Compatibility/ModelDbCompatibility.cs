using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class ModelDbCompatibility
{
    // safely try to get the ModelDb instance of the
    internal static CardModel? GetCardModelSafely(Type? cardType)
    {
        if (cardType is null) { return null; }
        var cardMethod = AccessTools.Method(
                typeof(ModelDb), nameof(ModelDb.Card)
            ).MakeGenericMethod([cardType]);
        return (CardModel?) cardMethod.Invoke(null, null);
    }

    internal static CardModel[] GetCardModelsSafely(Type?[] cardTypes)
    {
        CardModel[] cards = [];
        foreach (var cardType in cardTypes)
        {
            if (cardType is null) { continue; }

            var cardMethod = AccessTools.Method(
                typeof(ModelDb), nameof(ModelDb.Card)
            ).MakeGenericMethod([cardType]);
            var cardModel = cardMethod.Invoke(null, null);
            if (cardModel is null) { continue; }

            cards.Append(cardModel);
        }

        return cards;
    }
}


