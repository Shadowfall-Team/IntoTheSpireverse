using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.ui;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.Input;

internal static class ModShortcutHelpers
{
    // TODO: move back to publicizer once reflib bug is resolved
    internal static readonly Action<NClickableControl> OnPressHandler =
        AccessTools.MethodDelegate<Action<NClickableControl>>(
            AccessTools.Method(typeof(NClickableControl), "OnPressHandler"));

    internal static readonly Action<NClickableControl> OnReleaseHandler =
        AccessTools.MethodDelegate<Action<NClickableControl>>(
            AccessTools.Method(typeof(NClickableControl), "OnReleaseHandler"));

    internal static NCargoPile? GetCargoPile()
    {
        return NCombatRoom.Instance?.Ui
            .GetNode<NCombatPilesContainer>("%CombatPileContainer")
            ?.GetNodeOrNull<NCargoPile>("_CargoPile");
    }

    internal static NAmmoButton? GetAmmoButton()
    {
        return NCombatRoom.Instance?
            .GetCreatureNode(LocalContext.GetMe(RunManager.Instance.State.Players).Creature)
            ?.GetNodeOrNull<NAmmoButton>("AmmoButton");
    }
}