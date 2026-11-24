using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Ui;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches;

[HarmonyPatch(typeof(OrbCmd))]
internal static class OrbCmdSlotPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(OrbCmd.AddSlots))]
    internal static void AddSlotsPostfix(Player player)
    {
        UpdateAmmoButtonPosition(player);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OrbCmd.RemoveSlots))]
    internal static void RemoveSlotsPostfix(Player player)
    {
        UpdateAmmoButtonPosition(player);
    }

    private static void UpdateAmmoButtonPosition(Player player)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var ammoButton = creatureNode?.GetNodeOrNull<NShipDisplay>("AmmoButton");

        ammoButton?.ApplyOrbOffset(player.PlayerCombatState?.OrbQueue.Capacity > 0);
    }
}
