using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Ammo;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.AfterCardPlayed))]
public static class AmmoTargetTrackingPatch
{
    static void Prefix(CardPlay cardPlay)
    {
        if (cardPlay.Card.Type != CardType.Attack) return;
        if (cardPlay.Target is not { Side: CombatSide.Enemy } target) return;

        AmmoResource.SetLastAttackTarget(cardPlay.Player, target);
    }
}
