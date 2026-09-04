using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Modifications;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.Modifications;

/// <summary>
/// Applied by Ancestral Echo: playing the modified card also creates and plays copies of it.
///
/// The copies must be unmodified. CardModifier clones carry over onto card clones
/// (see CardModifier's clone handling), so a plain CreateClone would hand each copy this same
/// modification and the card would replay itself forever. Every Modification is stripped from the
/// copy before it is played.
/// </summary>
public sealed class AncestralEchoModification : Modification
{
    protected override ModelId SourceCardId => ModelDb.Card<AncestralEcho>().Id;

    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var source = Owner;
        if (source?.CombatState == null) return;

        for (var i = 0; i < Amount; i++)
        {
            var copy = source.CreateClone();

            foreach (var modification in DirectModifiers(copy).OfType<Modification>().ToList())
                RemoveModifier(copy, modification);

            await CardCmd.AutoPlay(choiceContext, copy, cardPlay.Target);
        }
    }
}
