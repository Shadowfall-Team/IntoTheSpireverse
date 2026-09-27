using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Powers;

/// <summary>
/// SlatePower decrements after the card's OnPlay, so a card cannot see its own spend there.
/// SlatePower calls this once the decrement has happened.
/// </summary>
public interface ISlateSpender
{
    Task OnSlateSpent(PlayerChoiceContext choiceContext, CardPlay cardPlay);
}
