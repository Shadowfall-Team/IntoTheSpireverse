using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Logging;

namespace IntoTheSpireverse.Patches;

[HarmonyPatch(typeof(OrbModel), nameof(OrbModel.CreateSprite))]
internal class CustomOrbCreateSprite
{
    [HarmonyPrefix]
    private static bool UseCustomCreateSprite(OrbModel __instance, ref Node2D __result)
    {
        if (!(__instance is IntoTheSpireverse.Orbs.CustomOrbModel customOrbModel))
            return true;

        string spritePath = customOrbModel.CustomSpritePath ?? 
            $"orbs/orb_visuals/{__instance.Id.Entry.ToLowerInvariant()}";

        Log.Info("SPRITE PATHH CREATION: " + spritePath);
        Node2D sprite = PreloadManager.Cache.GetScene(
            MegaCrit.Sts2.Core.Helpers.SceneHelper.GetScenePath(spritePath)
        ).Instantiate<Node2D>();
        
        new MegaSprite((Variant)(GodotObject)sprite.GetNode((NodePath)"SpineSkeleton"))
            .GetAnimationState()
            .SetAnimation("idle_loop");

        __result = sprite;
        return false;
    }
}