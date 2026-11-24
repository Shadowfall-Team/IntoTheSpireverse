using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Ui;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.CombatPiles;

[HarmonyPatch(typeof(NCombatPilesContainer))]
internal static class NCombatPilesContainerPatch
{
    [HarmonyPatch("Enable")]
    [HarmonyPostfix]
    internal static void EnablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCargoPile>("_CargoPile")?.Enable();
    }

    [HarmonyPatch("Disable")]
    [HarmonyPostfix]
    internal static void DisablePostfix(NCombatPilesContainer __instance)
    {
        __instance.GetNodeOrNull<NCargoPile>("_CargoPile")?.Disable();
    }
}

[HarmonyPatch(typeof(NCombatUi), "Activate")]
internal static class NCombatUiActivatePatch
{
    [HarmonyPostfix]
    internal static void ActivatePostfix(NCombatUi __instance, CombatState state)
    {
        var container = __instance.GetNode<NCombatPilesContainer>("%CombatPileContainer");
        var cargoPile = container.GetNodeOrNull<NCargoPile>("_CargoPile");
        var player = LocalContext.GetMe(state);
        if (player == null) return;

        cargoPile?.Initialize(player);

        var endTurnButton = NCombatRoom.Instance?.Ui.EndTurnButton;
        var ammoReminder = endTurnButton?.GetNodeOrNull<NAmmoCounterReminder>("_AmmoReminder");
        ammoReminder?.Initialize(player);
    }
}
