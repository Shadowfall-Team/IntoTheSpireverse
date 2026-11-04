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
            if (_parallelStoneMaxHpChange < 0)
                await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature,
                    -_parallelStoneMaxHpChange, isFromCard: false);
            else if (_parallelStoneMaxHpChange > 0)
                await CreatureCmd.GainMaxHp(Owner.Creature, _parallelStoneMaxHpChange);

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
    private int _parallelStoneMaxHpChange;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        if (Owner == null) return new List<EventOption>();

        _mirrorCharacterModel = Owner.Character is IAltCharacter ownerAltCharacter
            ? ownerAltCharacter.BaseCharacterModel
            : Owner.RunState.Rng.CombatCardSelection.NextItem(AltCharacterUtil.GetMirrorCharacters(Owner.Character));

        // The sign picks lose/gain wording (see ParallelStoneOption); the text itself only shows the size.
        _parallelStoneMaxHpChange = GetParallelStoneMaxHpChange(Owner.Character);
        DynamicVars.MaxHp.BaseValue = Math.Abs(_parallelStoneMaxHpChange);

        return
        [
            Option(TakeCards),
            ParallelStoneOption(),
            Option(UpgradeCard)
        ];
    }

    // Selects between one of three possible descriptions (lose, gain, or no Max HP change).
    private EventOption ParallelStoneOption()
    {
        const string key = "INTOTHESPIREVERSE-MIRROR_MIRROR.pages.INITIAL.options.TAKE_PARALLEL_STONE";
        var descriptionKey = _parallelStoneMaxHpChange switch
        {
            < 0 => $"{key}_LOSE_MAX_HP",
            > 0 => $"{key}_GAIN_MAX_HP",
            _ => key,
        };

        var option = new EventOption(this, TakeParallelStone, GetOptionTitle(key)!, GetOptionDescription(descriptionKey)!,
            key, HoverTipFactory.FromRelic<ParallelStone>());

        // Only a loss gets the usual "this will kill you" red warning (as seen in other max HP reduction events)
        return _parallelStoneMaxHpChange < 0 ? option.ThatDecreasesMaxHp(-_parallelStoneMaxHpChange) : option;
    }

    /// <summary>
    /// Negative loses Max HP, positive gains it, zero does neither. Checked in order:
    /// 1. A character that implements IParallelStoneCharacter uses its own value. We use this method for our own alts.
    /// 2. Otherwise an alt uses its base character's value. This is only a safety net for other modded alts such as the farmer Defect.
    ///    (it's reasonable to assume based on our first 3 characters that most of the time, the right tuning value is either equal
    ///    to or close to the same)
    /// 3. Otherwise a base-game character uses the table below.
    /// 4. Otherwise (only would theoretically trigger if a new base-game character is added), use the final fallback constant.
    /// </summary>
    public static int GetParallelStoneMaxHpChange(CharacterModel character) => character switch
    {
        IParallelStoneCharacter declared => declared.ParallelStoneMaxHpChange,
        IAltCharacter alt => GetParallelStoneMaxHpChange(alt.BaseCharacterModel),
        Ironclad => -15,
        Silent => -5,
        Defect => -5,
        Necrobinder => -5,
        Regent => 5,
        _ => -5, //final fallback constant
    };
}
