using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Shadowfall.ShadowfallCode.ui;

namespace Shadowfall.ShadowfallCode.Patches.CombatPiles;

[HarmonyPatch(typeof(NCombatPilesContainer))]
public static class NCombatPilesContainerPatch
{
    [HarmonyPatch("Enable")]
    [HarmonyPostfix]
    public static void EnablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCargoPile>("_CargoPile")?.Enable();
    }

    [HarmonyPatch("Disable")]
    [HarmonyPostfix]
    public static void DisablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCargoPile>("_CargoPile")?.Disable();
    }
    
    [HarmonyPatch(nameof(NCombatPilesContainer.Initialize))]
    [HarmonyPostfix]
    public static void Postfix(Player player)
    {
        if (!LocalContext.IsMe(player)) return;

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var ammoButton = creatureNode?.GetNodeOrNull<NAmmoButton>("AmmoButton");
        ammoButton?.Initialize(player);
    }
}

[HarmonyPatch(typeof(NCombatUi), "Activate")]
public static class NCombatUiActivatePatch
{
    [HarmonyPostfix]
    public static void ActivatePostfix(NCombatUi __instance, CombatState state)
    {
        var container = __instance.GetNode<NCombatPilesContainer>("%CombatPileContainer");
        var cargoPile = container.GetNodeOrNull<NCargoPile>("_CargoPile");
        var player = LocalContext.GetMe(state);
        cargoPile?.Initialize(player);
    }
}