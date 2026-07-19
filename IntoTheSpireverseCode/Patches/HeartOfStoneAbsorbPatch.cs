using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

/// Heart of Stone absorbs HP loss in ModifyHpLostAfterOstyLate, so by the time the game reaches
/// LoseHpInternal the amount is already gone and the resulting DamageResult reports 0 unblocked damage.
/// That keeps the HP intact (which is the point) but also silences every "took damage / lost HP" trigger
/// downstream — AfterDamageReceived, AfterCurrentHpChanged — so things like Clay Soldier never fire.
///
/// This patch re-reports the absorbed amount on the DamageResult the game hands back, so those hooks see
/// the damage that was dealt while the creature's HP is never actually touched.

[HarmonyPatch(typeof(Creature), nameof(Creature.LoseHpInternal))]
public static class HeartOfStoneAbsorbPatch
{
    private static Creature? _pendingCreature;
    private static int _pendingAmount;
    
    /// Called by <see cref="Relics.ShadowIronclad.HeartOfStone"/> once an absorb has actually been committed.
    /// The game calls LoseHpInternal immediately afterwards, which consumes this.
    public static void ReportAbsorbed(Creature creature, int amount)
    {
        _pendingCreature = creature;
        _pendingAmount = amount;
    }

    /// Drops any absorb that was recorded but never consumed by a LoseHpInternal call.
    public static void ClearPending()
    {
        _pendingCreature = null;
        _pendingAmount = 0;
    }

    public static void Postfix(Creature __instance, ref DamageResult __result)
    {
        if (_pendingCreature != __instance || _pendingAmount <= 0) return;

        var absorbed = _pendingAmount;
        ClearPending();

        // DamageResult's damage properties are init-only, so report the absorbed damage on a copy.
        __result = new DamageResult(__result.Receiver, __result.Props)
        {
            UnblockedDamage = __result.UnblockedDamage + absorbed,
            OverkillDamage = __result.OverkillDamage,
            WasTargetKilled = __result.WasTargetKilled,
            BlockedDamage = __result.BlockedDamage,
            WasBlockBroken = __result.WasBlockBroken,
            WasFullyBlocked = __result.WasFullyBlocked,
        };
    }
}
