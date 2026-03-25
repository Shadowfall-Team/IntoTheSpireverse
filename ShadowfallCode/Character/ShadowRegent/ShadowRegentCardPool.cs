using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.Cards.ShadowRegent;

namespace Shadowfall.ShadowfallCode.Character.ShadowRegent;

// TODO impl
public class ShadowRegentCardPool : CustomCardPoolModel
{
    public override string Title { get; }
    public override Color DeckEntryCardColor => new("ffffff");
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return new CardModel[]
        {
            ModelDb.Card<StrikeRegent>(),
            ModelDb.Card<DefendRegent>(),
            ModelDb.Card<Claim>(),
            ModelDb.Card<StarCharts>(),
            ModelDb.Card<FutureProofing>(),
            ModelDb.Card<CollisionCourse>(),
            ModelDb.Card<Charter>(),
            ModelDb.Card<Shadowfall.ShadowfallCode.Cards.ShadowRegent.CrescentSpear>(),
            ModelDb.Card<IceBeam>()
        };
    }
}