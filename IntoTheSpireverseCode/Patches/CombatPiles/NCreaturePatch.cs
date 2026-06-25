using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using IntoTheSpireverse.IntoTheSpireverseCode.ui;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.CombatPiles;

[HarmonyPatch(typeof(NCreature), "_Ready")]
public static class NCreaturePatch
{
    [HarmonyPostfix]
    public static void Postfix(NCreature __instance)
    {
        if (!__instance.Entity.IsPlayer) return;
        if (!LocalContext.IsMe(__instance.Entity.Player!)) return;

        var ammoButton = NAmmoButton.Create();
        ammoButton.Name = "AmmoButton";
        __instance.AddChild(ammoButton);
        ammoButton.Position = new Vector2(
            __instance.Hitbox.Size.X * 0.5f + 10f,
            -400f
        );
    }
}
