using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.Powers;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Singletons;

public class CargoDrawManager() : CustomSingletonModel(HookType.Combat)
{
     // BeforeHandDraw rather than AfterPlayerTurnStart: CombatManager runs this hook, then the
     // 5-card hand draw, then AfterPlayerTurnStart - so this is what puts Cargo cards in hand
     // ahead of the normal draw.
     public override async Task BeforeHandDraw(
         Player player,
         PlayerChoiceContext choiceContext,
         ICombatState combatState)
     {
         var cargoPile = CargoCardPile.CargoPileType.GetPile(player);
         if (!cargoPile.IsEmpty)
         {
             var tradeRoutes = player.Creature.GetPower<TradeRoutesPower>()?.Amount ?? 0;
             var cardModels = cargoPile.Cards.Take(1 + tradeRoutes).ToList();
             foreach (var cardModel in cardModels)
             {
                 if (PileType.Hand.GetPile(player).Cards.Count >= CardPile.MaxCardsInHand) return;

                 await CardPileCmd.Add(cardModel, PileType.Hand);
                 if (player.Creature.CombatState == null) continue;
                 await Hook.AfterCardDrawn(player.Creature.CombatState, choiceContext, cardModel, true);
             }
         }
     }
}