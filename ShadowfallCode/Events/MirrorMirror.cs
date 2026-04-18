using BaseLib.Abstracts;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.TopBar;
using MegaCrit.Sts2.Core.Runs;
using Shadowfall.ShadowfallCode.Character;

namespace Shadowfall.ShadowfallCode.Events;

//TODO: test if this works in MP
public sealed class MirrorMirror() : CustomEventModel(autoAdd: true)
{
    // public override string? CustomBackgroundScenePath => null;
    public override string? CustomInitialPortraitPath => "res://Shadowfall/images/card_portraits/card.png";
    // public override ActModel[] Acts => [ModelDb.Act<DefaultAct>()];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(6),
        new IntVar("CardSelect", 1),
        new MaxHpVar(3)
    ];

    public override bool IsAllowed(IRunState runState)
    {
        MainFile.Logger.Info($"MirrorMirror IsAllowed");
        return runState.Players.All(p =>
            p.Character is IAltCharacter ||
            ModelDb.AllCharacters.Any(a => a is IAltCharacter ac && ac.BaseCharacterModel == p.Character));
    }

    private async Task TakeCards()
    {
        if (Owner == null || _mirrorCharacterModel == null)
        {
            MainFile.Logger.Warn("Owner or MirrorChar is null");
        }
        else
        {
            await TakeCards(DynamicVars["CardSelect"].IntValue, DynamicVars.Cards.IntValue);
        }

        SetEventFinished(PageDescription("TOOK_CARDS"));
    }

    private async Task ReplaceCharacter()
    {
        if (Owner == null || _mirrorCharacterModel == null)
        {
            MainFile.Logger.Warn("Owner or MirrorChar is null");
        }
        else
        {
            await TakeCards(DynamicVars["CardSelect"].IntValue * 3, DynamicVars.Cards.IntValue * 3);
            await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue * 3);
            //TODO: replace starter relic

            var characterField = AccessTools.Field(typeof(Player), "<Character>k__BackingField");
            characterField.SetValue(Owner, _mirrorCharacterModel);

            // RefreshTopBarPortrait(_mirrorCharacterModel);
        }


        SetEventFinished(PageDescription("REPLACED_CHARACTER"));
    }

    private async Task TakeCards(int cardSelectCount, int cardCreateCount)
    {
        if (Owner == null)
        {
            MainFile.Logger.Warn("Owner is null");
            return;
        }

        var cardCreationResults = CardFactory.CreateForReward(
                Owner,
                cardCreateCount,
                CardCreationOptions.ForNonCombatWithUniformOdds([_mirrorCharacterModel.CardPool],
                        cardModel => !Owner.Character.CardPool.AllCards.Contains(cardModel))
                    .WithFlags(CardCreationFlags.NoRarityModification))
            .ToList();

        var cardSelectorPrefs =
            new CardSelectorPrefs(L10NLookup("SHADOWFALL-MIRROR_MIRROR.pages.TAKE_CARDS.selectionScreenPrompt"),
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
                .Where(c => c is IAltCharacter ac && ac.BaseCharacterModel == Owner.Character));
        return
        [
            Option(TakeCards),
            Option(ReplaceCharacter),
            Option(GainMaxHp)
        ];
    }
}