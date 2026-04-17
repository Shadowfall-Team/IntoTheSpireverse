using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using Shadowfall.ShadowfallCode.Cards.ShadowIronclad;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;
using Shadowfall.ShadowfallCode.Relics;
using Shadowfall.ShadowfallCode.Relics.ShadowIronclad;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowIronclad : PlaceholderCharacterModel
{
    public override string PlaceholderID => "ironclad";
    public const string CharacterId = "Shadowfall";

    public static readonly Color Color = StsColors.red;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 80;
    
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<StrikeShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<DefendShadowIronclad>(),
        ModelDb.Card<MadLad>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<CorruptBlood>()
    ];
    
    public override CardPoolModel CardPool => ModelDb.CardPool<ShadowIroncladCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShadowIroncladRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShadowIroncladPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    // public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}