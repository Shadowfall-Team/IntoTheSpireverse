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
        if (!__instance.Entity.IsPlayer)
        {
            // Anchor to HpBarContainer: the base game sizes it per-creature
            // (creature bounds + 12px each side) in UpdateLayoutForCreatureBounds,
            // so its rect matches each enemy's visible bar. The fixed-width hitbox
            // and the 0x0 NHealthBar rect do not.
            var stateDisplay = __instance._stateDisplay;
            var healthBar = stateDisplay?._healthBar;
            Control anchor = healthBar?.HpBarContainer
                             ?? stateDisplay?._hpBarHitbox
                             ?? (Control?)healthBar
                             ?? __instance.Hitbox;
            if (anchor == null) return;

            var indicator = NAmmoTargetIndicator.Create(__instance, anchor);
            indicator.Name = "AmmoTargetIndicator";
            __instance.AddChild(indicator);
            return;
        }

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
