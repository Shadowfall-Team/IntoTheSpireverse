using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;

public static class VfxCmdCompatibility
{
    static readonly MethodInfo? v111PlayOnCreatureCenter;
    static readonly MethodInfo? v113PlayOnCreatureCenter;
    static VfxCmdCompatibility()
    {
        v111PlayOnCreatureCenter = AccessTools.Method(typeof(VfxCmd), nameof(VfxCmd.PlayOnCreatureCenter),
            [typeof(Creature), typeof(string)]);
        v113PlayOnCreatureCenter = AccessTools.Method(typeof(VfxCmd), nameof(VfxCmd.PlayOnCreatureCenter),
            [typeof(Creature), typeof(string), typeof(bool)]);

        if ((v111PlayOnCreatureCenter ?? v113PlayOnCreatureCenter) is null)
        {
            throw new MethodAccessException("No valid signature for VfxCmd.PlayOnCreatureCenter found!");
        }
    }

    public static void PlayOnCreatureCenter(Creature target, string path, bool forceIfDead = true)
    {
        if (v111PlayOnCreatureCenter is not null)
        {
            v111PlayOnCreatureCenter.Invoke(null, [target, path]);
        }
        else
        {
            v113PlayOnCreatureCenter?.Invoke(null, [target, path, forceIfDead]);
        }
    }
}
