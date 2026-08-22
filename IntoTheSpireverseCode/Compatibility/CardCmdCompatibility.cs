using MegaCrit.Sts2.Core.Entities.Cards;
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
}