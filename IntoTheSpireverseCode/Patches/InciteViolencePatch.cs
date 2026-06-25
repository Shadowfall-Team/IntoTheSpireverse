using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(Hook), nameof(Hook.ModifyDamage))]
public static class InciteViolencePatch
{
    [ThreadStatic]
    public static bool IsIncitedAttack;

    public static bool Prefix(Decimal damage, ref Decimal __result, out IEnumerable<AbstractModel> modifiers)
    {
        if (!IsIncitedAttack)
        {
            modifiers = null!;
            return true;
        }

        modifiers = Enumerable.Empty<AbstractModel>();
        __result = Math.Max(0m, damage);
        return false;
    }
}