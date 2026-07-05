using System.Reflection;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class CompatibilityAnimation
{
    private const BindingFlags F = BindingFlags.Public | BindingFlags.Instance;

    private static MethodInfo? _setAnimationM;
    private static bool _initialized;
    private static bool _initFailed;

    private static bool EnsureInitialized()
    {
        if (_initialized) return !_initFailed;
        _initialized = true;
        try
        {
            _setAnimationM = FindByName(typeof(MegaAnimationState), "SetAnimation", typeof(string), typeof(bool));

            if (_setAnimationM == null)
                MainFile.Logger.Warn("CompatibilityAnimation: SetAnimation not found — animations will be skipped.");

            _initFailed = _setAnimationM == null;
        }
        catch (Exception ex)
        {
            _initFailed = true;
            MainFile.Logger.Warn($"CompatibilityAnimation: init failed, animations disabled. {ex.Message}");
        }
        return !_initFailed;
    }

    private static MethodInfo? FindByName(Type type, string name, params Type[] leading)
    {
        return type.GetMethods(F)
            .Where(m => m.Name == name)
            .Where(m =>
            {
                var ps = m.GetParameters();
                if (ps.Length < leading.Length) return false;
                for (var i = 0; i < leading.Length; i++)
                    if (ps[i].ParameterType != leading[i]) return false;
                for (var i = leading.Length; i < ps.Length; i++)
                    if (!ps[i].IsOptional) return false;
                return true;
            })
            .OrderBy(m => m.GetParameters().Length)
            .FirstOrDefault();
    }

    private static object? Call(MethodInfo m, object target, params object?[] args)
    {
        var ps = m.GetParameters();
        if (ps.Length == args.Length)
            return m.Invoke(target, args);

        var full = new object?[ps.Length];
        Array.Copy(args, full, args.Length);
        for (var i = args.Length; i < ps.Length; i++)
            full[i] = ps[i].DefaultValue;
        return m.Invoke(target, full);
    }

    // Log each distinct failure once, not every frame.
    private static readonly HashSet<string> LoggedFailures = [];
    private static void LogOnce(string key, string message)
    {
        if (LoggedFailures.Add(key))
            MainFile.Logger.Warn($"CompatibilityAnimation: {message}");
    }

    // ALWAYS use this instead of calling animState.SetAnimation directly — a direct call
    // bakes one version's signature into IL and JIT-crashes the entire containing method
    // on the other version.

    /// <summary>Version-safe SetAnimation. 107 returns an entry (disposed), 108 returns void.</summary>
    public static void SetAnimationCompat(this MegaAnimationState animState, string anim, bool loop = true)
    {
        if (!EnsureInitialized() || _setAnimationM == null) return;
        object? entry = null;
        try
        {
            entry = Call(_setAnimationM, animState, anim, loop);
        }
        catch (Exception ex)
        {
            LogOnce($"SetAnimationCompat:{ex.GetType().Name}", $"SetAnimation failed: {ex.InnerException?.Message ?? ex.Message}");
        }
        finally
        {
            try { (entry as IDisposable)?.Dispose(); } catch { }
        }
    }
}