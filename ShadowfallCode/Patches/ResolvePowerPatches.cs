using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Shadowfall.ShadowfallCode.Powers.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch]
public class ResolvePowerPatches
{
    [HarmonyPatch]
    public static class ResolveDamagePatch
    {
        [HarmonyPatch(typeof(Creature), nameof(Creature.LoseHpInternal))]
        [HarmonyPostfix]
        public static void Postfix(ref DamageResult __result)
        {
            if (ResolvePower.LastAbsorbed <= 0) return;

            var absorbed = (int)ResolvePower.LastAbsorbed;
            ResolvePower.LastAbsorbed = 0;

            __result = new DamageResult(__result.Receiver, __result.Props)
            {
                BlockedDamage = __result.BlockedDamage,
                UnblockedDamage = __result.UnblockedDamage + absorbed,
                OverkillDamage = __result.OverkillDamage,
                WasBlockBroken = __result.WasBlockBroken,
                WasFullyBlocked = __result.WasFullyBlocked,
                WasTargetKilled = __result.WasTargetKilled,
            };
        }
    }
}