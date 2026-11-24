using BaseLib.Abstracts;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.Spectrobinder;

public class Spectrobinder : PlaceholderCharacterModel, IAltCharacter, IParallelStoneCharacter, IIntoTheSpireverseDebug
{
    public override string PlaceholderID => "necrobinder";
    public const string CharacterId = "necrobinder";

    public static readonly Color Color = StsColors.purple;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;

    public override bool HideFromVanillaCharacterSelect => true;
    public override bool AllowInVanillaRandomCharacterSelect => true;

    public CharacterModel BaseCharacterModel => ModelDb.Character<Necrobinder>();
    public int ParallelStoneMaxHpChange => -5;
    public override int StartingHp => 66;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeNecrobinder>(),
        ModelDb.Card<StrikeNecrobinder>(),
        ModelDb.Card<StrikeNecrobinder>(),
        ModelDb.Card<StrikeNecrobinder>(),
        ModelDb.Card<DefendNecrobinder>(),
        ModelDb.Card<DefendNecrobinder>(),
        ModelDb.Card<DefendNecrobinder>(),
        ModelDb.Card<DefendNecrobinder>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<Circlet>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<SpectrobinderCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SpectrobinderRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SpectrobinderPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    // public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
