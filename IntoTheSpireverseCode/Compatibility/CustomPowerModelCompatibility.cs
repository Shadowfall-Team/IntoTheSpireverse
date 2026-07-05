using System.Reflection;
using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public abstract class CustomPowerModelCompatibility : CustomPowerModel
{
    public virtual decimal ModifyDamageAdditiveCompatibility(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 0;

    public virtual decimal ModifyDamageMultiplicativeCompatibility(Creature? target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 1;
}

internal static class PowerModifyDamagePatchHelper
{
    public static MethodBase Find(string name)
    {
        const BindingFlags f = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        Type[] oldSig = [typeof(Creature), typeof(decimal), typeof(ValueProp),
            typeof(Creature), typeof(CardModel)];

        return typeof(CustomPowerModelCompatibility).GetMethod(name, f, null, [.. oldSig, typeof(CardPlay)], null)
               ?? typeof(CustomPowerModelCompatibility).GetMethod(name, f, null, oldSig, null)
               ?? throw new MissingMethodException($"Power {name} not found in any known signature.");
    }
}

[HarmonyPatch]
internal static class PowerModifyDamageAdditivePatch
{
    private static MethodBase TargetMethod() => PowerModifyDamagePatchHelper.Find("ModifyDamageAdditive");

    [HarmonyPostfix]
    private static void Postfix(object __instance, object[] __args, ref decimal __result)
    {
        if (__instance is not CustomPowerModelCompatibility power) return;
        __result += power.ModifyDamageAdditiveCompatibility(
            (Creature?)__args[0], (decimal)__args[1], (ValueProp)__args[2],
            (Creature?)__args[3], (CardModel?)__args[4],
            __args.Length > 5 ? (CardPlay?)__args[5] : null);
    }
}

[HarmonyPatch]
internal static class PowerModifyDamageMultiplicativePatch
{
    private static MethodBase TargetMethod() => PowerModifyDamagePatchHelper.Find("ModifyDamageMultiplicative");

    [HarmonyPostfix]
    private static void Postfix(object __instance, object[] __args, ref decimal __result)
    {
        if (__instance is not CustomPowerModelCompatibility power) return;
        __result *= power.ModifyDamageMultiplicativeCompatibility(
            (Creature?)__args[0], (decimal)__args[1], (ValueProp)__args[2],
            (Creature?)__args[3], (CardModel?)__args[4],
            __args.Length > 5 ? (CardPlay?)__args[5] : null);
    }
}