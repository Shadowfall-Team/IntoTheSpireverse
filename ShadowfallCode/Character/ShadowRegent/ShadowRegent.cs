using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Character.ShadowRegent;

public class ShadowRegent : PlaceholderCharacterModel
{
    public override Color NameColor => StsColors.orange;
    public override CharacterGender Gender => CharacterGender.Masculine;
    
    public override int StartingHp => 75;
    
    public override CardPoolModel CardPool { get; }
    public override RelicPoolModel RelicPool { get; }
    public override PotionPoolModel PotionPool { get; }
    
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
    
    // TODO: impl
    public override IReadOnlyList<RelicModel> StartingRelics { get; }
}