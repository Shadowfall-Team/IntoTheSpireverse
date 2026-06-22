using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using Shadowfall.ShadowfallCode.Cards.ShadowIronclad;
using Shadowfall.ShadowfallCode.Extensions;
using Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowIronclad : PlaceholderCharacterModel, IAltCharacter
{
    public override string PlaceholderID => "ironclad";
    public const string CharacterId = "Shadowfall";

    public static readonly Color Color = StsColors.red;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    
    public override bool HideFromVanillaCharacterSelect => true;
    public override bool AllowInVanillaRandomCharacterSelect => true;
    
    public CharacterModel BaseCharacterModel => ModelDb.Character<Ironclad>();
    public override int StartingHp => 80;
    
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<TerraFirma>(),
        ModelDb.Card<Bore>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<HeartOfStone>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<ShadowIroncladCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShadowIroncladRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShadowIroncladPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override string CustomIconTexturePath => "character_icon_ironclad.png".GranitecladPath();
    public override string CustomCharacterSelectIconPath => "char_select_ironclad.png".GranitecladPath();
    public override string CustomArmPointingTexturePath => "multiplayer_hand_ironclad_point.png".GranitecladPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_ironclad_rock.png".GranitecladPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_ironclad_paper.png".GranitecladPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_ironclad_scissors.png".GranitecladPath();
    public override string CustomMapMarkerPath => "map_marker_ironclad.png".GranitecladPath();

    public override string CustomVisualPath => "res://Shadowfall/scenes/creature_visuals/shadowironclad.tscn";
    public override string CustomCharacterSelectBg => "res://Shadowfall/scenes/screens/char_select/shadowironclad.tscn";
    public override string CustomRestSiteAnimPath => "res://Shadowfall/scenes/rest_site/shadowironclad_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://Shadowfall/scenes/merchant/shadowironclad_merchant.tscn";

    // public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}