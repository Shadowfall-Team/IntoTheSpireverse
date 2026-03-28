using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Exceptions;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;

public sealed class BlastFromThePast() : ShadowNecrobinderCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1),
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

        var winningDeckCards = GetLastWinningRunCards();
        if (winningDeckCards == null || winningDeckCards.Count == 0) return;

        var candidates = winningDeckCards
            .UnstableShuffle(Owner.RunState.Rng.CombatCardGeneration)
            .Take(5)
            .Select(sc =>
            {
                var card = CardModel.FromSerializable(sc);
                card.Owner = Owner;
                return CombatState.CloneCard(card);
            })
            .ToList();

        if (candidates.Count == 0) return;

        int pickCount = IsUpgraded ? 2 : 1;
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, candidates, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, pickCount));

        foreach (var card in selected)
        {
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }

    // TODO Popup text if there is no winning run to reference from
    private List<SerializableCard>? GetLastWinningRunCards()
    {
        var runNames = SaveManager.Instance.GetAllRunHistoryNames()
            .OrderByDescending(n => n)
            .ToList();

        foreach (var runName in runNames)
        {
            var result = SaveManager.Instance.LoadRunHistory(runName);
            if (!result.Success) continue;
            if (!result.SaveData.Win) continue;

            var seen = new HashSet<ModelId>();
            var cards = new List<SerializableCard>();
            foreach (var player in result.SaveData.Players)
            {
                foreach (var card in player.Deck)
                {
                    if (!seen.Add(card.Id)) continue;
                    try
                    {
                        ModelDb.GetById<CardModel>(card.Id);
                        cards.Add(card);
                    }
                    catch (ModelNotFoundException) { }
                }
            }
            return cards;
        }
        return null;
    }
}