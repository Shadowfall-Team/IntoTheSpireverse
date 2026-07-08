using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.ui;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.CombatPiles;

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
        if (player == null) return;

        cargoPile?.Initialize(player);

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var ammoButton = creatureNode?.GetNodeOrNull<NAmmoButton>("AmmoButton");
        ammoButton?.Initialize(player);

        var endTurnButton = NCombatRoom.Instance?.Ui.EndTurnButton;
        var ammoReminder = endTurnButton?.GetNodeOrNull<NAmmoCounterReminder>("_AmmoReminder");
        ammoReminder?.Initialize(player);
    }
}