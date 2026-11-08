using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Extensions;

public static class CardPileCmdExtensions
{
    extension(CardPileCmd)
    {
        /// <summary>
        /// Same as <see cref="CardPileCmd.Draw(PlayerChoiceContext, Player)"/> but takes a target pile
        /// </summary>
        /// <param name="choiceContext">The context with which to handle player choices.</param>
        /// <param name="pileType">The pile to draw from.</param>
        /// <param name="player">Player who the hand and draw pile belongs to.</param>
        /// <returns>Card that was drawn, or null if no cards were drawn.</returns>
        /// <exception cref="InvalidOperationException">Thrown when used with <see cref="PileType.Draw"/>,
        /// use the regular <see cref="CardPileCmd.Draw(PlayerChoiceContext, Player)"/> command instead </exception>
        public static async Task<CardModel?> DrawFromPile(PlayerChoiceContext choiceContext, PileType pileType, Player player)
        {
            if (pileType is PileType.Draw) { throw new InvalidOperationException($"Don't use {nameof(DrawFromPile)} with {nameof(PileType.Draw)}."); }
            return (await DrawFromPile(choiceContext, pileType, 1, player)).FirstOrDefault();
        }

        /// <summary>
        /// Same as <see cref="CardPileCmd.Draw(PlayerChoiceContext, decimal, Player, bool)"/> but takes a target pile
        /// </summary>
        /// <param name="choiceContext">The context with which to handle player choices.</param>
        /// <param name="pileType">The pile to draw from.</param>
        /// <param name="count">Number of cards to draw.</param>
        /// <param name="player">Player who the hand and draw pile belongs to.</param>
        /// <param name="startOfTurnDraw">If this draw happened as part of the initial card draws at the start of your turn.</param>
        /// <returns>Cards that were drawn.</returns>
        /// <exception cref="InvalidOperationException">Thrown when used with <see cref="PileType.Draw"/>,
        /// use the regular <see cref="CardPileCmd.Draw(PlayerChoiceContext, decimal, Player, bool)"/> command instead </exception>
        public static Task<IEnumerable<CardModel>> DrawFromPile(PlayerChoiceContext choiceContext, PileType pileType, decimal count, Player player, bool startOfTurnDraw = false)
        {
            if (pileType is PileType.Draw) { throw new InvalidOperationException($"Don't use {nameof(DrawFromPile)} with {nameof(PileType.Draw)}."); }
            return DrawFromPileInternal(choiceContext, pileType, count, player, startOfTurnDraw);
        }

        /// <summary>
        /// Same as <see cref="CardPileCmd.DrawWithoutBlockingOnOtherPlayers(PlayerChoiceContext, decimal, Player, CardModel, bool)"/> but takes a target pile
        /// </summary>
        /// <param name="choiceContext">The context with which to handle player choices.</param>
        /// <param name="pileType">The pile to draw from.</param>
        /// <param name="count">Number of cards to draw.</param>
        /// <param name="player">Player who the hand and draw pile belongs to.</param>
        /// <param name="source">The source that is causing the draw.</param>
        /// <param name="fromHandDraw">If this draw happened as part of the initial card draws at the start of your turn.</param>
        /// <exception cref="InvalidOperationException">Thrown when used with <see cref="PileType.Draw"/>,
        /// use the regular <see cref="CardPileCmd.DrawWithoutBlockingOnOtherPlayers(PlayerChoiceContext, decimal, Player, CardModel, bool)"/> command instead </exception>
        public static Task DrawFromPileWithoutBlockingOnOtherPlayers(PlayerChoiceContext choiceContext, PileType pileType, decimal count, Player player, CardModel source, bool fromHandDraw = false)
        {
            if (pileType is PileType.Draw) { throw new InvalidOperationException($"Don't use {nameof(DrawFromPileWithoutBlockingOnOtherPlayers)} with {nameof(PileType.Draw)}."); }
            BranchingPlayerChoiceContext branchingPlayerChoiceContext = new BranchingPlayerChoiceContext(source, LocalContext.NetId.Value, GameActionType.Combat, choiceContext);
            Task<IEnumerable<CardModel>> task = DrawFromPile(branchingPlayerChoiceContext, pileType, count, player, fromHandDraw);
            return branchingPlayerChoiceContext.AssignTaskAndWaitForPauseOrCompletion(task);
        }


        // TODO: Keep an eye on this incase the game updates to make the draw command normally accept a pile
        // also need to test this fairly extensively for crossmod compat
        //
        // This may also work better as a harmony Reverse Patch that we run a transpiler on
        // to include the "ShouldDrawFromPile" hook call, so it, in theory, will update with versions
        private static async Task<IEnumerable<CardModel>> DrawFromPileInternal(PlayerChoiceContext choiceContext, PileType pileType, decimal count, Player player, bool startOfTurnDraw = false)
        {
            if (CombatManager.Instance.IsOverOrEnding) { return []; }

            var combatState = player.Creature.CombatState;
            if (combatState is null) { return []; }

            // Assume that something modifying drawing from the Draw Pile will also want to modify "drawing" from other piles (Fiddle, NoDrawPower)
            if (!Hook.ShouldDraw(combatState, player, startOfTurnDraw, out var preventer)) // CombatState shouldn't be null since the combat manager just told us we're in combat
            {
                await Hook.AfterPreventingDraw(combatState, preventer!); // preventer is always not-null when ShouldDraw returns false
                return [];
            }

            if (!IntoTheSpireverseHook.ShouldDrawFromPile(combatState, player, pileType, startOfTurnDraw, out preventer))
            {
                // should this be a new hook?
                await Hook.AfterPreventingDraw(combatState, preventer!);
                return [];
            }

            List<CardModel> result = [];
            var hand = PileType.Hand.GetPile(player);
            var pileToDrawFrom = pileType.GetPile(player);

            int drawsRequested = (int)Math.Max(Math.Ceiling(count), 0);
            if (drawsRequested == 0) { return result; }

            int freeHandSpaces = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
            if (freeHandSpaces <= 0 )
            {
                // CheckIfDrawIsPossibleFromPileAndShowThoughtBubbleIfNot(pileType, player);
                return result;
            }

            for (int i = 0; i < drawsRequested; i++)
            {
                if (freeHandSpaces <= 0 || CombatManager.Instance.IsOverOrEnding
                        /*|| !CheckIfDrawIsPossibleFromPileAndShowThoughtBubbleIfNot(pileType, player)*/)
                {
                    break;
                }
                // await ShufflePileIfNecessary(choiceContext, pile, player);
                // if (!CheckIfDrawIsPossibleFromPileAndShowThoughtBubbleIfNot(pileType, player)) { break ; }
                var card = pileToDrawFrom.Cards.FirstOrDefault();
                if (card == null || hand.Cards.Count >= CardPile.MaxCardsInHand) { break; }

                result.Add(card);
                await CardPileCmd.Add(card, hand);
                CombatManager.Instance.History.CardDrawnFromPile(combatState, card, pileType, startOfTurnDraw);
                // TODO: May need to make another hook for this that passes the pile it was drawn from
                await Hook.AfterCardDrawn(combatState, choiceContext, card, startOfTurnDraw);
                card.InvokeDrawn();
                NDebugAudioManager.Instance?.Play(TmpSfx.cardDeal, 0.25f, PitchVariance.Small);
                freeHandSpaces = Math.Max(0, CardPile.MaxCardsInHand - hand.Cards.Count);
            }

            return result;
        }
    }
}
