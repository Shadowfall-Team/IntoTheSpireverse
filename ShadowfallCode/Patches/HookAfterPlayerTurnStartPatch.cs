using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Hooks;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Patches;

[HarmonyPatch(typeof(Hook), "AfterPlayerTurnStart", MethodType.Async)]
public class HookAfterPlayerTurnStartPatch
{
    static void Postfix(Player ___player)
    {
        var cargoPile = CargoCardPile.CargoPileType.GetPile(___player);
        if (!cargoPile.IsEmpty)
        {
            var tradeRoutes = ___player.Creature.GetPower<TradeRoutesPower>()?.Amount ?? 0;
            var cardModels = cargoPile.Cards.Take(1 + tradeRoutes).ToList();
            if (cardModels.Count != 0)
            {
                CardPileCmd.Add(cardModels, PileType.Hand);
            }
        }
    }
}
