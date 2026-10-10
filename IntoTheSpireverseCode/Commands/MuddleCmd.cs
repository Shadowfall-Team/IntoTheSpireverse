using System.Reflection;
using BaseLib.Extensions;
using BaseLib.Utils;
using BaseLib.Utils.ModInterop;
using GodotPlugins.Game;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Random;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Commands;

public static class MuddleCmd
{
    // Downfall Muddle compatability
    /// <summary>
    /// Ease of access cache of call `ModManager.GetLoadedMods().FirstOrDefault(m => m.manifest?.id == "Downfall");`
    /// </summary>
    private static bool IsDownfallLoaded;
    private const string _DownfallMuddleInterfaceNamespace = "Snecko.SneckoCode.Core";

    // These interface names should remain the same across mods, but switch to hardcoded strings if they change for whatever reason
    private const string _DownfallMaxMuddleCostListenerName = $"{_DownfallMuddleInterfaceNamespace}.{nameof(IMaxMuddleCost)}";
    private const string _DownfallAfterMuddleListenerName = $"{_DownfallMuddleInterfaceNamespace}.{nameof(IAfterCardMuddled)}";
    private const string _DownfallShouldPermanentMuddleListenerName = $"{_DownfallMuddleInterfaceNamespace}.{nameof(IShouldPermanentMuddleListener)}";

    private static Type? Downfall_MaxMuddleCostListener;
    private static MethodInfo? Downfall_ModifyMaxMuddleCostMethod;
    private static MethodInfo? Downfall_AfterModifyingMaxMuddleCostMethod;

    private static Type? Downfall_AfterMuddleListener;
    private static MethodInfo? Downfall_AfterCardMuddledMethod;

    private static Type? Downfall_ShouldPermanentMuddleListener;
    private static MethodInfo? Downfall_ShouldPermanentMuddleMethod;
    private static MethodInfo? Downfall_AfterShouldPermanentMuddleMethod;

    /// <summary>
    /// Call after all mods are loaded.
    /// Attempts to get the references to the muddle methods and hooks;
    /// </summary>
    internal static void TryFindDownfallMuddleListeners()
    {
        var downfall = ModManager.GetLoadedMods().FirstOrDefault(m => m.manifest?.id == "Downfall");
        if (downfall is null)
        {
            MainFile.Logger.Info("Downfall not found in mod list, skipping Muddle compatabality registration");
            return;
        }

        IsDownfallLoaded = true;
        var sneckoAssembly = downfall.assemblies.FirstOrDefault(a => a.FullName.Contains("snecko", StringComparison.OrdinalIgnoreCase));
        if (sneckoAssembly is null)
        {
            MainFile.Logger.Info("Snecko is turned off, skipping Muddle compatabality registration");
            return;
        }

        Downfall_AfterMuddleListener = sneckoAssembly.GetType(_DownfallAfterMuddleListenerName);
        Downfall_AfterCardMuddledMethod = Downfall_AfterMuddleListener?.GetMethod(nameof(IAfterCardMuddled.AfterCardMuddled));

        Downfall_MaxMuddleCostListener = sneckoAssembly.GetType(_DownfallMaxMuddleCostListenerName);
        Downfall_ModifyMaxMuddleCostMethod = Downfall_AfterMuddleListener?.GetMethod(nameof(IMaxMuddleCost.AfterModifyingMaxMuddleCost));
        Downfall_AfterModifyingMaxMuddleCostMethod = Downfall_MaxMuddleCostListener?.GetMethod(nameof(IMaxMuddleCost.AfterModifyingMaxMuddleCost));

        Downfall_ShouldPermanentMuddleListener = sneckoAssembly.GetType(_DownfallShouldPermanentMuddleListenerName);
        Downfall_ShouldPermanentMuddleMethod = Downfall_ShouldPermanentMuddleListener?.GetMethod(nameof(IShouldPermanentMuddleListener.ShouldPermanentMuddle));
        Downfall_AfterShouldPermanentMuddleMethod = Downfall_ShouldPermanentMuddleListener?.GetMethod(nameof(IShouldPermanentMuddleListener.AfterShouldPermanentMuddle));
    }


    public static bool CanMuddle(CardModel card)
    {
        return !card.Keywords.Contains(CardKeyword.Unplayable)
               && !card.EnergyCost.CostsX;
    }

    private const int MaxMuddleCost = 3;

    // TODO: All of this really wants to use the baselib helper syntax, but it needs an overload to accept a
    // generic type to check against, and i'm not up for that right now
    private static async Task AfterCardMuddled(ICombatState cs, PlayerChoiceContext ctx, CardModel card )
    {
        foreach (var model in cs.IterateHookListeners())
        {
            if (model is IAfterCardMuddled muddleModel)
            {
                await muddleModel.AfterCardMuddled(ctx, card);
                continue;
            }
            if (IsDownfallLoaded && model.GetType().IsAssignableTo(Downfall_AfterMuddleListener))
            {
                MainFile.Logger.Debug($"model {model.GetType().Name} is {Downfall_AfterMuddleListener.Name}, attempting to call hook");
                if (Downfall_AfterCardMuddledMethod is not null)
                {
                    await (Task)Downfall_AfterCardMuddledMethod.Invoke(model, [ctx, card]);
                }
            }
        }
    }

    private static async Task<int> GetMaxMuddleCost(ICombatState cs, PlayerChoiceContext ctx, CardModel card)
    {
        // just here to reduce typo chance
        var listenerType = Downfall_MaxMuddleCostListener;
        var listenerMethod = Downfall_ModifyMaxMuddleCostMethod;
        var listenerMethodAfter = Downfall_AfterModifyingMaxMuddleCostMethod;

        int cost = MaxMuddleCost;
        var hl = cs.IterateHookListeners();
        foreach (var model in hl)
        {
            if (model is IMaxMuddleCost maxCostModel)
            {
                cost = maxCostModel.ModifyMaxMuddleCost(card, cost);
                continue;
            }
            if (IsDownfallLoaded && model.GetType().IsAssignableTo(listenerType))
            {
                MainFile.Logger.Debug($"model {model.GetType().Name} is {listenerType.Name}, attempting to call hook");
                if (listenerMethod is not null)
                {
                    cost = await (Task<int>)listenerMethod.Invoke(model, [card, cost]);
                }
            }
        }

        foreach (var model in hl)
        {
            if (model is IMaxMuddleCost maxCostModel)
            {
                await maxCostModel.AfterModifyingMaxMuddleCost(ctx, card);
                model.InvokeExecutionFinished();
                continue;
            }
            if (IsDownfallLoaded && model.GetType().IsAssignableTo(listenerType))
            {
                MainFile.Logger.Debug($"model {model.GetType().Name} is Downfall_AfterMuddleListener, attempting to call hook");
                if (listenerMethodAfter is not null)
                {
                    await (Task)listenerMethodAfter.Invoke(model, [card, cost]);
                    model.InvokeExecutionFinished();
                }
            }
        }
        // var a = HookUtils.Modify<IMaxMuddleCost, int>(cs, MaxMuddleCost,
        //     (m, i) => m.ModifyMaxMuddleCost(card, i), out var modifiers);
        // await HookUtils.AfterModifying(cs, modifiers, m => m.AfterModifyingMaxMuddleCost(ctx, card));
        return cost;
    }

    private static async Task<bool> ShouldPermanentMuddle(ICombatState cs, PlayerChoiceContext ctx, CardModel card)
    {
        // just here to reduce typo chance
        var listenerType = Downfall_ShouldPermanentMuddleListener;
        var listenerMethod = Downfall_ShouldPermanentMuddleMethod;
        var listenerMethodAfter = Downfall_AfterShouldPermanentMuddleMethod;

        bool shouldPermanentMuddle = false;
        var hl = cs.IterateHookListeners();
        foreach (var model in hl)
        {
            if (model is IShouldPermanentMuddleListener permanentMuddleModel)
            {
                shouldPermanentMuddle |= permanentMuddleModel.ShouldPermanentMuddle(card);
                continue;
            }
            if (IsDownfallLoaded && model.GetType().IsAssignableTo(listenerType))
            {
                MainFile.Logger.Debug($"model {model.GetType().Name} is {listenerType.Name}, attempting to call hook");
                if (listenerMethod is not null)
                {
                    shouldPermanentMuddle |= await (Task<bool>)listenerMethod.Invoke(model, [card]);
                }
            }
        }

        foreach (var model in hl)
        {
            if (model is IShouldPermanentMuddleListener permanentMuddleModel)
            {
                await permanentMuddleModel.AfterShouldPermanentMuddle(ctx, card);
                model.InvokeExecutionFinished();
                continue;
            }
            if (IsDownfallLoaded && model.GetType().IsAssignableTo(listenerType))
            {
                MainFile.Logger.Debug($"model {model.GetType().Name} is {listenerType.Name}, attempting to call hook");
                if (listenerMethodAfter is not null)
                {
                    await (Task)listenerMethodAfter.Invoke(model, [ctx, card]);
                    model.InvokeExecutionFinished();
                }
            }
        }
        // var permanentMuddle = HookUtils.Any<IShouldPermanentMuddleListener>(
        //     cs, e => e.ShouldPermanentMuddle(card), out var matches);
        // await HookUtils.AfterModifying(cs, matches, e => e.AfterShouldPermanentMuddle(ctx, card));
        return shouldPermanentMuddle;
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

    public static async Task<CardModel?> ApplyMuddle(PlayerChoiceContext ctx, CardModel card, bool lowerOnly = false)
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