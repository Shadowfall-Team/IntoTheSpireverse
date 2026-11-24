using BaseLib.Abstracts;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Config;
using IntoTheSpireverse.IntoTheSpireverseCode.Extensions;
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

    public override string CustomIconPath => "res://IntoTheSpireverse/scenes/character_icons/spectrobinder_icon.tscn";
    public override string CustomIconTexturePath => "character_icon_spectrobinder.png".SpectrobinderPath();
    public override string CustomCharacterSelectIconPath => "char_select_spectrobinder.png".SpectrobinderPath();
    public override string CustomArmPointingTexturePath => "multiplayer_hand_spectrobinder_point.png".SpectrobinderPath();
    public override string CustomArmRockTexturePath => "multiplayer_hand_spectrobinder_rock.png".SpectrobinderPath();
    public override string CustomArmPaperTexturePath => "multiplayer_hand_spectrobinder_paper.png".SpectrobinderPath();
    public override string CustomArmScissorsTexturePath => "multiplayer_hand_spectrobinder_scissors.png".SpectrobinderPath();
    public override string CustomMapMarkerPath => "map_marker_spectrobinder.png".SpectrobinderPath();

    public override string CustomVisualPath => "res://IntoTheSpireverse/scenes/creature_visuals/spectrobinder.tscn";
    public override string CustomCharacterSelectBg => "res://IntoTheSpireverse/scenes/screens/char_select/spectrobinder.tscn";
    public override string CustomRestSiteAnimPath => "res://IntoTheSpireverse/scenes/rest_site/spectrobinder_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://IntoTheSpireverse/scenes/merchant/spectrobinder_merchant.tscn";
}
