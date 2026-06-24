using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.utils;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Events;

//TODO: test if this works in MP
public sealed class MirrorMirror() : CustomEventModel(autoAdd: true)
{
    public override string CustomInitialPortraitPath => "res://IntoTheSpireverse/images/events/mirror_mirror.png";
    public override string CustomVfxPath =>"res://scenes/vfx/events/doors_of_light_and_dark_vfx.tscn";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("TakeCardsSelect", 1),
        new IntVar("TakeCardsCount", 6),

        new IntVar("ReplaceCharSelect", 3),
        new IntVar("ReplaceCharCount", 18),
        new IntVar("ReplaceCharMaxHp", 9),

        new MaxHpVar(3)
    ];

    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All(p =>
            p.Character is IAltCharacter ||
            ModelDb.AllCharacters.Any(a => AltCharacterUtil.IsAvailableAltCharacter(a) && a is IAltCharacter ac && ac.BaseCharacterModel == p.Character));
    }

    private async Task TakeCards()
    {
        if (Owner != null && _mirrorCharacterModel != null)
        {
            await TakeCards(DynamicVars["TakeCardsSelect"].IntValue, DynamicVars["TakeCardsCount"].IntValue);
        }

        SetEventFinished(PageDescription("TOOK_CARDS"));
    }

    private async Task ReplaceCharacter()
    {
        if (Owner != null && _mirrorCharacterModel != null)
        {
            await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue * 3);

            await TakeCards(DynamicVars["ReplaceCharSelect"].IntValue, DynamicVars["ReplaceCharCount"].IntValue,
                "REPLACED_CHARACTER");

            var relicModels = Owner.Relics.Where(r => r.Rarity == RelicRarity.Starter).ToList();
            foreach (var relic in relicModels)
            {
                await RelicCmd.Remove(relic);
            }

            var characterField = AccessTools.Field(typeof(Player), "<Character>k__BackingField");
            characterField.SetValue(Owner, _mirrorCharacterModel);

            for (var index = 0; index < Owner.Character.StartingRelics.Count; index++)
            {
                var relicModel = Owner.Character.StartingRelics[index];
                await RelicCmd.Obtain(relicModel.ToMutable(), Owner, index);
            }
        }

        SetEventFinished(PageDescription("REPLACED_CHARACTER"));
    }

    private async Task TakeCards(int cardSelectCount, int cardCreateCount, string action = "TAKE_CARDS")
    {
        if (Owner == null || _mirrorCharacterModel == null) return;

        var cardCreationResults = CardFactory.CreateForReward(
                Owner,
                cardCreateCount,
                CardCreationOptions.ForNonCombatWithDefaultOdds([_mirrorCharacterModel.CardPool],
                    Owner.Character is IAltCharacter
                        ? cardModel => !Owner.Character.CardPool.AllCards.Contains(cardModel)
                        : null)
            )
            .OrderByDescending(c => c.Card.Rarity)
            .ThenBy(c => c.Card.Id)
            .ToList();

        var cardSelectorPrefs =
            new CardSelectorPrefs(L10NLookup($"INTOTHESPIREVERSE-MIRROR_MIRROR.pages.{action}.selectionScreenPrompt"),
                cardSelectCount);

        var selectedCards =
            await CardSelectCmd.FromSimpleGridForRewards(new BlockingPlayerChoiceContext(), cardCreationResults, Owner,
                cardSelectorPrefs);
        foreach (var cardModel in selectedCards)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(cardModel, PileType.Deck));
        }
    }

    private async Task GainMaxHp()
    {
        if (Owner == null) return;
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
        SetEventFinished(PageDescription("GAINED_MAX_HP"));
    }

    private CharacterModel? _mirrorCharacterModel;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        if (Owner == null) return new List<EventOption>();

        _mirrorCharacterModel = Owner.Character is IAltCharacter ownerAltCharacter
            ? ownerAltCharacter.BaseCharacterModel
            : Owner.RunState.Rng.CombatCardSelection.NextItem(ModelDb.AllCharacters
                .Where(c => AltCharacterUtil.IsAvailableAltCharacter(c) && c is IAltCharacter ac && ac.BaseCharacterModel == Owner.Character));
        return
        [
            Option(TakeCards),
            Option(ReplaceCharacter),
            Option(GainMaxHp)
        ];
    }
}