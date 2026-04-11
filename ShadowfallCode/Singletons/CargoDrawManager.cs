//TODO: readd when singletonmodels are properly supported (and remove CombatManagerStartTurnPatch).
// see https://discord.com/channels/309399445785673728/1478917653551906947/1487458524790264070 for more details
// see https://github.com/Alchyr/BaseLib-StS2/commit/677306eb3af5353d0ec073c5c5842b43168bcda9 for implementation details

// using MegaCrit.Sts2.Core.Commands;
// using MegaCrit.Sts2.Core.Entities.Cards;
// using MegaCrit.Sts2.Core.Entities.Players;
// using MegaCrit.Sts2.Core.GameActions.Multiplayer;
// using MegaCrit.Sts2.Core.Models;
// using Shadowfall.ShadowfallCode.CardPiles;
//
// namespace Shadowfall.ShadowfallCode.Singletons;
//
// public class CargoDrawManager : SingletonModel
// {
//     public override bool ShouldReceiveCombatHooks => true;
//
//     public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
//     {
//         var cargoPile = CargoCardPile.CargoPileType.GetPile(player);
//         if (!cargoPile.IsEmpty)
//         {
//             var card = cargoPile.Cards.FirstOrDefault();
//             if (card != null)
//             {
//                 await CardPileCmd.Add(card, PileType.Hand);
//             }
//         }
//     }
// }