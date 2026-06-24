using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using IntoTheSpireverse.IntoTheSpireverseCode.Relics;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class ShadowSilent : PlaceholderCharacterModel, IAltCharacter, IIntoTheSpireverseDebug
{
    public override string PlaceholderID => "silent";
    public const string CharacterId = "IntoTheSpireverse";

    public static readonly Color Color = StsColors.blue;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    
    public override bool HideFromVanillaCharacterSelect => true;
    public override bool AllowInVanillaRandomCharacterSelect => true;
    
    public CharacterModel BaseCharacterModel => ModelDb.Character<Silent>();
    
    public override int StartingHp => 75;
    
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeShadowSilent>(),
        ModelDb.Card<StrikeShadowSilent>(),
        ModelDb.Card<StrikeShadowSilent>(),
        ModelDb.Card<StrikeShadowSilent>(),
        ModelDb.Card<DefendShadowSilent>(),
        ModelDb.Card<DefendShadowSilent>(),
        ModelDb.Card<DefendShadowSilent>(),
        ModelDb.Card<DefendShadowSilent>(),
        ModelDb.Card<CheapShot>(),
        ModelDb.Card<MeasuredDefense>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ArmoredPack>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<ShadowSilentCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShadowSilentRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShadowSilentPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    // public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}