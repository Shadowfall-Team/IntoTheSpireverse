using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using Shadowfall.ShadowfallCode.Cards.ShadowNecrobinder;
using Shadowfall.ShadowfallCode.Relics;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowNecrobinder : PlaceholderCharacterModel
{
    public override string PlaceholderID => "necrobinder";
    public const string CharacterId = "Shadowfall";

    public static readonly Color Color = StsColors.purple;

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
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
        ModelDb.Card<ClenchFist>(),
        ModelDb.Card<Servitude>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<Lantern>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<ShadowNecrobinderCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShadowNecrobinderRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShadowNecrobinderPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    // public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    // public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    // public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
