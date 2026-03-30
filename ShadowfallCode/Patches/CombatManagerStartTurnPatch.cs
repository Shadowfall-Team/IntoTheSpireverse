using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.CardPiles;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(Hook), "AfterPlayerTurnStart", MethodType.Async)]
public class CombatManagerStartTurnPatch
{
    static void Postfix(Player ___player)
    {
        var cargoPile = CargoCardPile.CargoPileType.GetPile(___player);
        if (!cargoPile.IsEmpty)
        {
            var card = cargoPile.Cards.FirstOrDefault();
            if (card != null)
            {
                CardPileCmd.Add(card, PileType.Hand);
            }
        }
    }
}
