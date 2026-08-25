using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.ControllerInput;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using IntoTheSpireverse.IntoTheSpireverseCode.Ui;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.Input;

[HarmonyPatch(typeof(NButton), "_Input")]
public static class NButtonPatches
{
    [HarmonyPrefix]
    public static bool InputPrefix(NButton __instance, InputEvent inputEvent)
    {
        if (__instance is not NCharacterSelectButton nCharacterSelectButton) return true;
        if (!__instance.IsVisibleInTree()) return true;

        if (inputEvent.IsActionReleased(MegaInput.up))
        {
            if (!nCharacterSelectButton.IsSelected) return true;

            var arrow = nCharacterSelectButton.GetChildren().OfType<NCharAltArrow>().FirstOrDefault();
            if (arrow != null)
            {
                arrow.DoPress();
                return false;
            }
        }

        return true;
    }
}