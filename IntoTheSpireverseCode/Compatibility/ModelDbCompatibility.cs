using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class ModelDbCompatibility
{
    internal static CardModel[] GetCardModelsSafely(Type?[] cardTypes)
    {

        MainFile.Logger.Debug($"{cardTypes}");

        List<CardModel> cards = [];
        foreach (var cardType in cardTypes)
        {
            if (cardType == null) { continue; }
            MainFile.Logger.Debug(cardType.ToString());

            var cardMethod = AccessTools.Method(typeof(ModelDb), nameof(ModelDb.Card));
            var genericMethod = cardMethod.MakeGenericMethod([cardType]);
            var cardModel = genericMethod.Invoke(null, null);
            if (cardModel == null) { continue; }
            MainFile.Logger.Debug($"{genericMethod}");
            MainFile.Logger.Debug($"{cardModel}");

            cards.Add((CardModel)cardModel);
        }

        return cards.ToArray<CardModel>();
    }
}


