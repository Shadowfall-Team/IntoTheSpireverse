using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.ui;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(OrbCmd))]
public static class OrbCmdSlotPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(OrbCmd.AddSlots))]
    public static void AddSlotsPostfix(Player player)
    {
        UpdateAmmoButtonPosition(player);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OrbCmd.RemoveSlots))]
    public static void RemoveSlotsPostfix(Player player)
    {
        UpdateAmmoButtonPosition(player);
    }

    private static void UpdateAmmoButtonPosition(Player player)
    {
        if (!LocalContext.IsMe(player)) return;
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var ammoButton = creatureNode?.GetNodeOrNull<NAmmoButton>("AmmoButton");

        ammoButton?.ApplyOrbOffset(player.PlayerCombatState?.OrbQueue.Capacity > 0);
    }
}
