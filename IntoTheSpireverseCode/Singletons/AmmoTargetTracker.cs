using BaseLib.Abstracts;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Singletons;

/// <summary>
/// Remembers the last enemy the player aimed an Attack at, so Ammo shots default to that
/// enemy instead of picking a random one.
/// </summary>
public class AmmoTargetTracker() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack) return Task.CompletedTask;
        if (cardPlay.Target is not { Side: CombatSide.Enemy } target) return Task.CompletedTask;

        AmmoResource.SetLastAttackTarget(cardPlay.Player, target);
        return Task.CompletedTask;
    }
}
