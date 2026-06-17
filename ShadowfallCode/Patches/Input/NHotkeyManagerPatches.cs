using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Shadowfall.ShadowfallCode.ui;
using GodotInput = Godot.Input;

namespace Shadowfall.ShadowfallCode.Patches.Input;

[HarmonyPatch(typeof(NHotkeyManager), nameof(NHotkeyManager._UnhandledInput))]
public static class NHotkeyManagerPatches
{
    private static bool _comboConsumedLeftTrigger;
    private static bool _comboConsumedRightTrigger;

    [HarmonyPrefix]
    public static bool UnhandledInputPrefix(InputEvent inputEvent)
    {
        if (_comboConsumedLeftTrigger && inputEvent.IsActionReleased(MegaInput.viewDrawPile, exactMatch: false))
        {
            _comboConsumedLeftTrigger = false;
            return false;
        }

        if (_comboConsumedRightTrigger && inputEvent.IsActionReleased(MegaInput.viewDiscardPile, exactMatch: false))
        {
            _comboConsumedRightTrigger = false;
            return false;
        }

        if (inputEvent.IsActionPressed(MegaInput.viewDiscardPile, exactMatch: false)
            && GodotInput.IsActionPressed(MegaInput.viewDrawPile))
        {
            _comboConsumedLeftTrigger = true;
            _comboConsumedRightTrigger = true;

            var container = NCombatRoom.Instance?.Ui.GetNode<NCombatPilesContainer>("%CombatPileContainer");
            var cargoPile = container?.GetNodeOrNull<NCargoPile>("_CargoPile");

            if (cargoPile != null && cargoPile.IsVisibleInTree())
            {
                cargoPile.OnPressHandler();
                cargoPile.OnReleaseHandler();
            }

            return false;
        }

        if (inputEvent.IsActionPressed(MegaInput.select, exactMatch: false)
            && GodotInput.IsActionPressed(Controller.leftTrigger))
        {
            _comboConsumedLeftTrigger = true;

            var localCreature = LocalContext.GetMe(RunManager.Instance.State.Players).Creature;
            var ammoButton = NCombatRoom.Instance?.GetCreatureNode(localCreature)
                ?.GetNodeOrNull<NAmmoButton>("AmmoButton");
            if (ammoButton != null && ammoButton.IsVisibleInTree())
            {
                ammoButton.OnPressHandler();
                ammoButton.OnReleaseHandler();
            }

            return false;
        }

        return true;
    }
}