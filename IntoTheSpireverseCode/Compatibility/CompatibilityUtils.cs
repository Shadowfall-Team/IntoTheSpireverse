using System.Reflection;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class CompatibilityUtils
{
    
    private static readonly Dictionary<Type, MethodInfo?> IgnoreNextInstanceCache = [];
    internal static void DoHackyThingsForSpecificPowers(ITemporaryPower power)
    {

        var type = power.GetType();
        if (!IgnoreNextInstanceCache.TryGetValue(type, out var m))
        {
            m = type.GetMethod("IgnoreNextInstance",
                BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
            IgnoreNextInstanceCache[type] = m;
        }
        m?.Invoke(power, null);
    }
}