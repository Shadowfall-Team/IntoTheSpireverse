using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Commands;

public static class MuddleCmd
{
    public static bool CanMuddle(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable)
               && !card.EnergyCost.CostsX;
    }
    
    private const int MaxMuddleCost = 3;
    
    
    private static async Task<int> GetMaxMuddleCost(ICombatState cs, PlayerChoiceContext ctx, CardModel card)
    {
        var a = HookUtils.Modify<IMaxMuddleCost, int>(cs, MaxMuddleCost,
            (m, i) => m.ModifyMaxMuddleCost(card, i), out var modifiers);
        await HookUtils.AfterModifying(cs, modifiers, m => m.AfterModifyingMaxMuddleCost(ctx, card));
        return a;
    }
    
    private static async Task<bool> ShouldPermanentMuddle(ICombatState cs, PlayerChoiceContext ctx, CardModel card)
    {
        var permanentMuddle = HookUtils.Any<IShouldPermanentMuddleListener>(
            cs, e => e.ShouldPermanentMuddle(card), out var matches);
        await HookUtils.AfterModifying(cs, matches, e => e.AfterShouldPermanentMuddle(ctx, card));
        return permanentMuddle;
    }
    
    private static Task AfterCardMuddled(ICombatState cs, PlayerChoiceContext ctx, CardModel card )
    {
        return HookUtils.Dispatch<IAfterCardMuddled>(cs, m => m.AfterCardMuddled(ctx, card));
    }
    
    private static int RollMuddleCost(int maxCost, int currentCost, Rng rng)
    {
        if (maxCost <= 0)
            return 0;
        if (currentCost > maxCost)
            return rng.NextInt(maxCost + 1);
        var roll = rng.NextInt(maxCost);
        return roll >= currentCost ? roll + 1 : roll;
    }

    public static async Task<CardModel?> ApplyMuddle(PlayerChoiceContext ctx, CardModel card,
        bool lowerOnly = false)
    {
        var cs = card.Owner.Creature.CombatState;
        if (cs == null) return null;
        if (!CanMuddle(card))
            return null;
        
        var maxCost = await GetMaxMuddleCost(cs, ctx, card);
        var currentCost = card.EnergyCost.GetWithModifiers(CostModifiers.All);
        if (lowerOnly)
        {
            maxCost = Math.Min(maxCost, currentCost-1);
        }
        var newCost = RollMuddleCost(maxCost, currentCost, card.Owner.RunState.Rng.CombatEnergyCosts);


        var permanentMuddle = await ShouldPermanentMuddle(cs, ctx, card);
        if (permanentMuddle)
            card.EnergyCost.SetThisCombat(newCost);
        else
            card.EnergyCost.SetThisTurnOrUntilPlayed(newCost);

        NCard.FindOnTable(card)?.PlayRandomizeCostAnim();
        
        await AfterCardMuddled(cs, ctx, card);
        return card;
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleAll(
        PlayerChoiceContext choiceContext,
        IEnumerable<CardModel> cards)
    {
        List<CardModel> results = [];
        foreach (var card in cards)
        {
            if (await ApplyMuddle(choiceContext, card) is { } muddled)
                results.Add(muddled);
        }
        return results;
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleFromHandSelection(
        PlayerChoiceContext choiceContext,
        Player player,
        AbstractModel source,
        int count = 1)
    {
        var prefs = new CardSelectorPrefs(
            new LocString("card_selection", "INTOTHESPIREVERSE-MUDDLE_PROMPT"),
            count, count
        );
        var selected = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            CanMuddle,
            source
        );
        return await ApplyMuddleAll(choiceContext, selected);
    }


    public static async Task<IEnumerable<CardModel>> ApplyMuddleHand(PlayerChoiceContext choiceContext, Player player)
    {
        return await ApplyMuddleAll(choiceContext, PileType.Hand.GetPile(player).Cards.Where(CanMuddle));
    }

    public static async Task<IEnumerable<CardModel>> ApplyMuddleRandom(
        PlayerChoiceContext choiceContext, Player player, int count, Rng rng)
    {
        var eligible = PileType.Hand.GetPile(player).Cards
            .Where(CanMuddle)
            .ToList();

        List<CardModel> muddled = [];
        while (muddled.Count < count && eligible.Count > 0)
        {
            var index = rng.NextInt(eligible.Count);
            var card = eligible[index];
            eligible.RemoveAt(index);
            if (await ApplyMuddle(choiceContext, card) is { } result)
                muddled.Add(result);
        }
        return muddled;
    }
}

internal interface IMaxMuddleCost
{
    int ModifyMaxMuddleCost(CardModel card, int i);
    Task AfterModifyingMaxMuddleCost(PlayerChoiceContext choiceContext, CardModel card);
}

public interface IAfterCardMuddled
{
    Task AfterCardMuddled(PlayerChoiceContext choiceContext, CardModel cardModel);
}

public interface IShouldPermanentMuddleListener
{
    bool ShouldPermanentMuddle(CardModel card);
    Task AfterShouldPermanentMuddle(PlayerChoiceContext choiceContext, CardModel card);
}