using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Keywords;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch]
public static class CardPileCmdPatch
{
    // [HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add), [typeof(CardModel), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool)])]
    // [HarmonyPrefix]
    public static bool NoAddIfLinger(CardModel card, CardPile newPile)
    {
        if (card.Keywords.Contains(ShadowfallKeywords.Linger))
        {
            MainFile.Logger.Info("Hey, that's a Linger trigger!");
            if (newPile.Type == PileType.Discard)
            {
                return false;
            }
        }
        return true;
    }

    // Transpiler here in CombatManager.DoTurnEnd, add extra return for ShadowfallKeywords.Linger
    // public static void 
}
