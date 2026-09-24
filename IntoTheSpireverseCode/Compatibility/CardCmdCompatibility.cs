using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public class CardCmdCompatibility
{
    public static void ApplySingleTurnRetain(CardModel card)
    {
        card.GiveSingleTurnRetain();
        if (card.Pile != null) NCard.FindOnTable(card)?.UpdateVisuals(card.Pile.Type, CardPreviewMode.Normal);
    }

    // Exhaust command compat
    // This is probably more prone to breakage than lamali's impl, but i'm not sure where it would actually break?
    // (see https://github.com/lamali292/Downfall/blob/main/DownfallCode/Compatibility/CardCmdCompatibility.cs)
    private static MethodInfo? _exhaustMethod;
    private static MethodInfo? ExhaustMethod
    {
        get
        {
            _exhaustMethod ??= AccessTools.Method(typeof(CardCmd), nameof(CardCmd.Exhaust),
                [typeof(PlayerChoiceContext), typeof(CardModel), typeof(bool), typeof(bool)])
                ?? throw new MissingMethodException("CardCmd.Exhaust overload not recognised");
            return _exhaustMethod;
        }
    }

    public static async Task<CardPileAddResult?> Exhaust(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal = false, bool skipVisuals = false)
    {
        if (ExhaustMethod?.ReturnType == typeof(Task<CardPileAddResult?>))
        {
            return await (Task<CardPileAddResult?>)ExhaustMethod.Invoke(null, [choiceContext, card, causedByEthereal, skipVisuals]);
        }

        if (ExhaustMethod?.ReturnType.IsAssignableTo(typeof(Task)) == true)
        {
            await (Task)ExhaustMethod.Invoke(null, [choiceContext, card, causedByEthereal, skipVisuals]);
            return null;
        }

        throw new MissingMethodException($"CardCmd.Exhaust return type not expected - {_exhaustMethod.ReturnType}");
    }
}