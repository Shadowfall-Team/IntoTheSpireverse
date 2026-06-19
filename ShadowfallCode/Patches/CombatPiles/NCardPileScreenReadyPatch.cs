using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Patches.CombatPiles;

[HarmonyPatch(typeof(NCardPileScreen), "_Ready")]
public static class NCardPileScreenReadyPatch
{
    [HarmonyPostfix]
    public static void ReadyPrefix(NCardPileScreen __instance)
    {
        if (__instance.Pile.Type != CargoCardPile.CargoPileType) return;

        __instance._bottomLabel.Text = "[center]" + new LocString("gameplay_ui", "SHADOWFALL-CARGO_PILE_INFO").GetFormattedText();
        __instance._bottomLabel.Visible = true;
    }
}