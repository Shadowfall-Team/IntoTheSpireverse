using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using GodotInput = Godot.Input;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.Input;

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

            var cargoPile = ModShortcutHelpers.GetCargoPile();
            if (cargoPile == null || !cargoPile.IsVisibleInTree()) return false;

            ModShortcutHelpers.OnPressHandler(cargoPile);
            ModShortcutHelpers.OnReleaseHandler(cargoPile);

            return false;
        }

        if (inputEvent.IsActionPressed(MegaInput.select, exactMatch: false)
            && GodotInput.IsActionPressed(Controller.leftTrigger))
        {
            _comboConsumedLeftTrigger = true;

            var ammoButton = ModShortcutHelpers.GetAmmoButton();
            if (ammoButton == null || !ammoButton.IsVisibleInTree()) return false;

            ModShortcutHelpers.OnPressHandler(ammoButton);
            ModShortcutHelpers.OnReleaseHandler(ammoButton);

            return false;
        }

        return true;
    }
}