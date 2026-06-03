using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Shadowfall.ShadowfallCode.ui;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(NCombatPilesContainer), nameof(NCombatPilesContainer.Initialize))]
public static class NCombatPilesContainerInitPatch
{
    [HarmonyPostfix]
    public static void Postfix(Player player)
    {
        if (!LocalContext.IsMe(player)) return;

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        var ammoButton = creatureNode?.GetNodeOrNull<NAmmoButton>("AmmoButton");
        ammoButton?.Initialize(player);
    }
}
