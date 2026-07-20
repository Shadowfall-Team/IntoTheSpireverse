using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using IntoTheSpireverse.IntoTheSpireverseCode.CardPiles;
using IntoTheSpireverse.IntoTheSpireverseCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Singletons;

public class ArcaneSingleton() : CustomSingletonModel(HookType.Combat)
{
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (combatState.RoundNumber != 1)
            return;
        foreach (CardPile pile in player.Piles) 
        {
            if (pile.Type == PileType.Draw)
            {
                foreach (CardModel card in pile.Cards.ToList())
                {
                    if (card.Keywords.Contains(IntoTheSpireverseKeywords.Arcane))
                    {
                        await CardPileCmd.Add(card, PileType.Exhaust);
                    }
                }
                
            }
        }
    }
}