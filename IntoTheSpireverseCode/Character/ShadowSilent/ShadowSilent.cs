using BaseLib.Abstracts;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent.Relics;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent;

public class ShadowSilent : PlaceholderCharacterModel, IAltCharacter, IIntoTheSpireverseDebug
{
    public override string PlaceholderID => "silent";
    public const string CharacterId = "IntoTheSpireverse";

    public static readonly Color Color = StsColors.blue;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    
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
        ModelDb.Card<Foretell>(),
        ModelDb.Card<Clothesline>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<MysticCharm>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<ShadowSilentCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShadowSilentRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShadowSilentPotionPool>();
    
    
    public override string CustomArmPointingTexturePath => "multiplayer_hand_silent_point.png".ShadowSilentPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_silent_rock.png".ShadowSilentPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_silent_paper.png".ShadowSilentPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_silent_scissors.png".ShadowSilentPath();
    
    public override string CustomIconTexturePath => "character_icon_silent.png".ShadowSilentPath();
    public override string? CustomIconOutlineTexturePath => "character_icon_silent_outline.png".ShadowSilentPath();
    public override string CustomCharacterSelectIconPath => "char_select_silent.png".ShadowSilentPath();
    public override string CustomMapMarkerPath => "map_marker_silent.png".ShadowSilentPath();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    // public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    
    public override string CustomVisualPath => "res://IntoTheSpireverse/scenes/creature_visuals/shadowsilent.tscn";
    public override string CustomMerchantAnimPath => "res://IntoTheSpireverse/scenes/merchant/shadowsilent_merchant.tscn";
    public override string CustomCharacterSelectBg => "res://IntoTheSpireverse/scenes/screens/char_select/shadowsilent.tscn";
    public override string CustomRestSiteAnimPath => "res://IntoTheSpireverse/scenes/rest_site/shadowsilent_rest_site.tscn";
}