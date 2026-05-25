using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Shadowfall.ShadowfallCode.ui;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(NCreature), "_Ready")]
public static class NCreaturePatch
{
    [HarmonyPostfix]
    public static void Postfix(NCreature __instance)
    {
        if (!__instance.Entity.IsPlayer) return;

        var player = __instance.Entity.Player!;
        if (!LocalContext.IsMe(player)) return;

        var ammoButton = NAmmoButton.Create();
        ammoButton.Name = "AmmoButton";
        ammoButton.Position = new Vector2(
            __instance.Hitbox.Size.X * 0.5f + 55f,
            -400f
        );
        ammoButton.SetPlayer(player);
        __instance.AddChild(ammoButton);
    }
}
