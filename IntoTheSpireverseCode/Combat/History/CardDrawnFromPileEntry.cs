using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Combat.History;

public class CardDrawnFromPileEntry(CardModel card, PileType pile, bool startOfTurnDraw,
    int roundNumber, CombatSide currentSide, CombatHistory history, IEnumerable<Player> players)
    : CombatHistoryEntry(card.Owner.Creature, roundNumber, currentSide, history, players)
{
    public override string Description => $"{Actor.Player?.Character.Id.Entry} drew {Card.Id.Entry} "
        + $"from {CustomEnums.EnumName<PileType>((int)Pile)}";
    public PileType Pile = pile;
    public CardModel Card = card;
    public bool StartOfTurnDraw = startOfTurnDraw;
}
