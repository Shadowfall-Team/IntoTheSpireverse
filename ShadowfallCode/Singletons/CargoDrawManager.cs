using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Shadowfall.ShadowfallCode.CardPiles;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Singletons;

public class CargoDrawManager() : CustomSingletonModel(true, false)
{
     public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
     {
         var cargoPile = CargoCardPile.CargoPileType.GetPile(player);
         if (!cargoPile.IsEmpty)
         {
             var tradeRoutes = player.Creature.GetPower<TradeRoutesPower>()?.Amount ?? 0;
             var cardModels = cargoPile.Cards.Take(1 + tradeRoutes).ToList();
             if (cardModels.Count != 0)
             {
                 await CardPileCmd.Add(cardModels, PileType.Hand);
             }
         }
     }
}