using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// Implemented by cards that pay off when playing them spends Slate. SlatePower decrements in
/// AfterCardPlayed, after the card's own OnPlay has finished, so a card cannot observe its own spend;
/// SlatePower calls this instead once the decrement has actually happened.
/// </summary>
public interface ISlateSpender
{
    Task OnSlateSpent(PlayerChoiceContext choiceContext, CardPlay cardPlay);
}
