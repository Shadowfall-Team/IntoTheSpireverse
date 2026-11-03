using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Runs;
using ShadowIroncladCharacter = IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad.ShadowIronclad;
using ShadowSilentCharacter = IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.ShadowSilent;
using ShadowDefectCharacter = IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect.ShadowDefect;
using ShadowNecrobinderCharacter = IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowNecrobinder.ShadowNecrobinder;
using ShadowRegentCharacter = IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent.ShadowRegent;
using IntoTheSpireverse.IntoTheSpireverseCode.Character;
using IntoTheSpireverse.IntoTheSpireverseCode.Relics;
using IntoTheSpireverse.IntoTheSpireverseCode.Utils;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Events;

//TODO: test if this works in MP
public sealed class MirrorMirror() : CustomEventModel(autoAdd: true)
{
    public override string CustomInitialPortraitPath => "res://IntoTheSpireverse/images/events/mirror_mirror.png";
    public override string CustomVfxPath =>"res://scenes/vfx/events/doors_of_light_and_dark_vfx.tscn";

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("TakeCardsSelect", 2),
        new IntVar("TakeCardsCount", 12),
        new MaxHpVar(0m),

        new CardsVar(1)
    ];

    public override bool IsAllowed(IRunState runState)
    {
        return runState.CurrentActIndex < 2
               && runState.Players.All(p => AltCharacterUtil.HasMirrorCharacter(p.Character));
    }

    private async Task TakeCards()
    {
        if (Owner != null && _mirrorCharacterModel != null)
        {
            await TakeCards(DynamicVars["TakeCardsSelect"].IntValue, DynamicVars["TakeCardsCount"].IntValue);
        }

        SetEventFinished(PageDescription("TOOK_CARDS"));
    }

    private async Task TakeParallelStone()
    {
        if (Owner != null)
        {
            await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature,
                DynamicVars.MaxHp.BaseValue, isFromCard: false);
            await RelicCmd.Obtain<ParallelStone>(Owner);
        }

        SetEventFinished(PageDescription("TOOK_PARALLEL_STONE"));
    }

    private async Task TakeCards(int cardSelectCount, int cardCreateCount, string action = "TAKE_CARDS")
    {
        if (Owner == null || _mirrorCharacterModel == null) return;

        // Never offer a card we already have our own copy of: an outright duplicate of one of our cards, or (for alt
        // characters, whose pool borrows from the base character's) a card that is in both pools already.
        var bannedIds = AltCharacterUtil.GetBannedCardIds(Owner.Character);
        var isAlt = Owner.Character is IAltCharacter;
        var ownPoolCards = Owner.Character.CardPool.AllCards;

        var cardCreationResults = CardFactory.CreateForReward(
                Owner,
                cardCreateCount,
                CardCreationOptions.ForNonCombatWithDefaultOdds([_mirrorCharacterModel.CardPool],
                    cardModel => !bannedIds.Contains(cardModel.Id) &&
                                 (!isAlt || !ownPoolCards.Contains(cardModel)))
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

    private async Task UpgradeCard()
    {
        if (Owner != null)
        {
            var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, DynamicVars.Cards.IntValue);
            foreach (var cardModel in await CardSelectCmd.FromDeckForUpgrade(Owner, prefs))
            {
                CardCmd.Upgrade(cardModel);
            }
        }

        SetEventFinished(PageDescription("UPGRADED_CARD"));
    }

    private CharacterModel? _mirrorCharacterModel;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        if (Owner == null) return new List<EventOption>();

        _mirrorCharacterModel = Owner.Character is IAltCharacter ownerAltCharacter
            ? ownerAltCharacter.BaseCharacterModel
            : Owner.RunState.Rng.CombatCardSelection.NextItem(AltCharacterUtil.GetMirrorCharacters(Owner.Character));

        DynamicVars.MaxHp.BaseValue = GetParallelStoneMaxHpLoss(Owner.Character);

        return
        [
            Option(TakeCards),
            Option(TakeParallelStone, HoverTipFactory.FromRelic<ParallelStone>())
                .ThatDecreasesMaxHp(DynamicVars.MaxHp.BaseValue),
            Option(UpgradeCard)
        ];
    }

    private static int GetParallelStoneMaxHpLoss(CharacterModel character) => character switch
    {
        Ironclad => 10,
        ShadowIroncladCharacter => 10,
        Silent => 6,
        ShadowSilentCharacter => 6,
        Defect => 8,
        ShadowDefectCharacter => 8,
        Necrobinder => 6,
        ShadowNecrobinderCharacter => 6,
        Regent => 2,
        ShadowRegentCharacter => 2,
        _ => 6,
    };
}
