using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Combat.History;
using IntoTheSpireverse.IntoTheSpireverseCode.Combat.History;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

public static class CombatHistoryExtensions
{
    extension(CombatHistory combatHistory)
    {
        public void CardDrawnFromPile(ICombatState combatState, CardModel card, PileType pile, bool startOfTurnDraw)
        {
            combatHistory.Add(combatState, new CardDrawnFromPileEntry(card, pile, startOfTurnDraw, combatState.RoundNumber, combatState.CurrentSide, combatHistory, combatState.Players));
        }
    }
}
