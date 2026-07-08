using Godot;
using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using MegaCrit.Sts2.Core.Nodes.Combat;
using IntoTheSpireverse.IntoTheSpireverseCode.ui;
namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.CombatPiles;

[HarmonyPatch(typeof(NEndTurnButton), "_Ready")]
public static class NEndTurnButtonPatch
{
    [HarmonyPostfix]
    public static void Postfix(NEndTurnButton __instance)
    {
        if (!IntoTheSpireverseConfig.ShowAmmoReminder) return;

        var reminder = new NAmmoCounterReminder();
        reminder.Name = "_AmmoReminder";
        __instance.AddChild(reminder);
        reminder.Position = new Vector2(175, -50f);
    }
}
