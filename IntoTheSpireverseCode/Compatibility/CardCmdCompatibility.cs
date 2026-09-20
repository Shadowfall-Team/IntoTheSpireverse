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
    public static async Task<CardPileAddResult?> Exhaust(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal = false, bool skipVisuals = false)
    {
        var classInfo = typeof(CardCmd);
        var exhaustMethod = AccessTools.Method(classInfo, nameof(CardCmd.Exhaust),
            [typeof(PlayerChoiceContext), typeof(CardModel), typeof(bool), typeof(bool)])
            ?? throw new MissingMethodException("CardCmd.Exhaust overload not recognised");

        var returnType = exhaustMethod.ReturnType;
        if (returnType == typeof(Task<CardPileAddResult>))
        {
            return await (Task<CardPileAddResult>)exhaustMethod.Invoke(null, [choiceContext, card, causedByEthereal, skipVisuals]);
        }
        else if (returnType == typeof(Task))
        {
            await (Task)exhaustMethod.Invoke(null, [choiceContext, card, causedByEthereal, skipVisuals]);
            return null;
        }
        else
        {
            return null;
        }
    }
}