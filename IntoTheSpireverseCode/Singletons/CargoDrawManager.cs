using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;
using IntoTheSpireverse.IntoTheSpireverseCode.Powers.ShadowRegent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Singletons;

public class CargoDrawManager() : CustomSingletonModel(true, false)
{
     public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
     {
         var cargoPile = CargoCardPile.CargoPileType.GetPile(player);
         if (!cargoPile.IsEmpty)
         {
             var tradeRoutes = player.Creature.GetPower<TradeRoutesPower>()?.Amount ?? 0;
             var cardModels = cargoPile.Cards.Take(1 + tradeRoutes).ToList();
             foreach (var cardModel in cardModels)
             {
                 await CardPileCmd.Add(cardModel, PileType.Hand);
                 if (player.Creature.CombatState == null) continue;
                 await Hook.AfterCardDrawn(player.Creature.CombatState, choiceContext, cardModel, true);
             }
         }
     }
}