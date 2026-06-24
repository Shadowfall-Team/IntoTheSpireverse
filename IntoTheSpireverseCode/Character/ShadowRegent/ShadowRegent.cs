using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Characters;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;
using IntoTheSpireverse.IntoTheSpireverseCode.Relics.ShadowRegent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class ShadowRegent : PlaceholderCharacterModel, IAltCharacter
{
    public override string PlaceholderID => "regent";
    public const string CharacterId = "regent";

    public static readonly Color Color = StsColors.orange;
    public override Color NameColor => Color;
    public override Color EnergyLabelOutlineColor => new("784000FF");
    public override Color DialogueColor => new("52371D");
    public override Color MapDrawingColor => new("935206");
    public override Color RemoteTargetingLineColor => new("BFA270FF");
    public override Color RemoteTargetingLineOutline => new("784000FF");
    
    public override CharacterGender Gender => CharacterGender.Masculine;

    // public override string? CustomCharacterSelectIconPath => "IntoTheSpireverse/images/charui/char_select_char_name.png";

    public override bool HideFromVanillaCharacterSelect => true;
    public override bool AllowInVanillaRandomCharacterSelect => true;
    
    public CharacterModel BaseCharacterModel => ModelDb.Character<Regent>();

    public override int StartingHp => 75;
    
    public override CardPoolModel CardPool => ModelDb.CardPool<ShadowRegentCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShadowRegentRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShadowRegentPotionPool>();
    
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeRegent>(),
        ModelDb.Card<StrikeRegent>(),
        ModelDb.Card<StrikeRegent>(),
        ModelDb.Card<StrikeRegent>(),
        ModelDb.Card<DefendRegent>(),
        ModelDb.Card<DefendRegent>(),
        ModelDb.Card<DefendRegent>(),
        ModelDb.Card<DefendRegent>(),
        ModelDb.Card<StarCharts>(),
        ModelDb.Card<Load>(),
    ];
    
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<CaptainsHat>()
    ];

    public override string CustomIconTexturePath => "character_icon_regent.png".ShadowRegentPath();
    public override string CustomCharacterSelectIconPath => "char_select_regent.png".ShadowRegentPath();
    public override string CustomArmPointingTexturePath => "multiplayer_hand_regent_point.png".ShadowRegentPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_regent_rock.png".ShadowRegentPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_regent_paper.png".ShadowRegentPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_regent_scissors.png".ShadowRegentPath();
    public override string CustomMapMarkerPath => "map_marker_regent.png".ShadowRegentPath();

    public override string CustomVisualPath => "res://IntoTheSpireverse/scenes/creature_visuals/shadowregent.tscn";
    public override string CustomCharacterSelectBg => "res://IntoTheSpireverse/scenes/screens/char_select/shadowregent.tscn";
    public override string CustomRestSiteAnimPath => "res://IntoTheSpireverse/scenes/rest_site/shadowregent_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://IntoTheSpireverse/scenes/merchant/shadowregent_merchant.tscn";

}