using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;
using Shadowfall.ShadowfallCode.Relics.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowRegent : PlaceholderCharacterModel
{
    public override string PlaceholderID => "regent";
    public const string CharacterId = "Shadowfall";
    public static readonly Color Color = StsColors.orange;
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    
    public override int StartingHp => 75;
    
    // TODO: impl
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
        ModelDb.Card<Claim>(),
    ];
    
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SpareBullet>()
    ];
}