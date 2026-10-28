using HarmonyLib;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Patches.CombatPiles;

[HarmonyPatch(typeof(NCardPileScreen), "_Ready")]
public static class NCardPileScreenReadyPatch
{
    [HarmonyPostfix]
    public static void ReadyPrefix(NCardPileScreen __instance)
    {
        if (__instance.Pile.Type != CargoCardPile.CargoPileType) return;

        __instance._bottomLabel.Text = "[center]" + new LocString("gameplay_ui", "INTOTHESPIREVERSE-CARGO_PILE_INFO").GetFormattedText();
        __instance._bottomLabel.Visible = true;
    }
}
