using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using Shadowfall.ShadowfallCode.Relics;
using Decay = Shadowfall.ShadowfallCode.Cards.ShadowDefect.Decay;
using Invoke = Shadowfall.ShadowfallCode.Cards.ShadowDefect.Invoke;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowDefect : PlaceholderCharacterModel, IAltCharacter
{
    public override string PlaceholderID => "defect";
    public const string CharacterId = "Shadowfall";

    public static readonly Color Color = StsColors.blue;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    
    public override bool HideFromVanillaCharacterSelect => true;
    public override bool AllowInVanillaRandomCharacterSelect => true;
    
    public CharacterModel BaseCharacterModel => ModelDb.Character<Defect>();
    
    public override int StartingHp => 75;
    
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeDefect>(),
        ModelDb.Card<StrikeDefect>(),
        ModelDb.Card<StrikeDefect>(),
        ModelDb.Card<StrikeDefect>(),
        ModelDb.Card<DefendDefect>(),
        ModelDb.Card<DefendDefect>(),
        ModelDb.Card<DefendDefect>(),
        ModelDb.Card<DefendDefect>(),
        ModelDb.Card<Invoke>(),
        ModelDb.Card<Decay>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<CorruptedCore>()
    ];
    
    public override int BaseOrbSlotCount => 3;
    
    public override CardPoolModel CardPool => ModelDb.CardPool<ShadowDefectCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShadowDefectRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShadowDefectPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    // public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}