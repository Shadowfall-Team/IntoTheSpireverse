using System.Reflection;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class CompatibilityOrb
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    // old: public void Trigger() / new: protected void ActivatePassive()
    private static readonly MethodInfo? TriggerM =
        typeof(OrbModel).GetMethod("Trigger", F, null, Type.EmptyTypes, null)
        ?? typeof(OrbModel).GetMethod("ActivatePassive", F, null, Type.EmptyTypes, null);

    /// <summary>
    /// Version-safe orb trigger signal. Old builds: Trigger(). New builds: ActivatePassive().
    /// No-ops (rather than crashing) if neither exists on a future build.
    /// </summary>
    public static void TriggerCompat(this OrbModel orb)
        => TriggerM?.Invoke(orb, null);
}