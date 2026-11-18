using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Interfaces;

public interface IDrawFromPileModifier
{
    /// <summary>
    /// Modify the amount of cards the player should draw at the start of their turn.
    /// </summary>
    /// <param name="pileType"><see cref="PileType"/> that cards will be drawn from.</param>
    /// <param name="player">Player who will draw the cards.</param>
    /// <param name="originalCardCount">Original number of cards that would be drawn.</param>
    /// <returns>New amount of cards to be drawn.</returns>
    public decimal ModifyStartOfTurnDrawFromPile(PileType pileType, Player player, decimal originalCardCount);

    /// <summary>
    /// Runs after the number of cards drawn at the beginning of the player's turn was modified.
    /// Combat-only hook.
    /// </summary>
    /// <param name="pileType"><see cref="PileType"/> that had its "draw" modified.</param>
    public Task AfterModifyingStartOfTurnDrawFromPile(PileType pileType);
}
