using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Helpers;
using Godot;
using IntoTheSpireverse.Orbs;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(OrbModel), "SpritePath", MethodType.Getter)]
public class OrbModelPatch
{
    /// <summary>
    /// Postfix patch to modify the SpritePath property getter return value.
    /// Returns a custom path for EntropyOrb, otherwise uses the original default path.
    /// </summary>
    static void Postfix(OrbModel __instance, ref string __result)
    {
        if (__instance == null)
            return;
        
        if (__instance is EntropyOrb)
        {
            __result = SceneHelper.GetScenePath("orbs/orb_visuals/lightning_orb");
        }
    }
}