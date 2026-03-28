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
            ModelDb.Card<Cards.ShadowRegent.CrescentSpear>(),
            ModelDb.Card<IceBeam>(),
            ModelDb.Card<GuidingShot>(),
            ModelDb.Card<StowAway>(),
            ModelDb.Card<Cards.ShadowRegent.SolarStrike>(),
            ModelDb.Card<PoweredBeam>(),
            ModelDb.Card<BeneathMe>(),
            ModelDb.Card<RoyalCloak>(),
            ModelDb.Card<KnowThyPlace>(),
            ModelDb.Card<Plot>(),
            ModelDb.Card<Cards.ShadowRegent.GatherLight>(),
            ModelDb.Card<Cards.ShadowRegent.Glow>(),
            ModelDb.Card<Cards.ShadowRegent.HiddenCache>(),
            ModelDb.Card<Patter>(),
            ModelDb.Card<FireAway>(),
            ModelDb.Card<Reload>(),
            ModelDb.Card<Glitterstream>(),
            ModelDb.Card<SmugglersStrike>(),
            ModelDb.Card<LunarBlast>(),
            ModelDb.Card<Jettison>(),
            ModelDb.Card<KnowledgeBlast>(),
            ModelDb.Card<IllicitMunitions>(),
            ModelDb.Card<KinglyPunch>(),
            ModelDb.Card<ShadowCrystal>(),
            ModelDb.Card<Supermassive>(),
            ModelDb.Card<Energize>(),
            ModelDb.Card<Strongarm>(),
            ModelDb.Card<KinglyKick>(),
            ModelDb.Card<FillTheTank>(),
            ModelDb.Card<FirstOfficer>(),
            ModelDb.Card<Monologue>(),
            ModelDb.Card<ReinforcedBody>(),
            ModelDb.Card<Banana>(),
        };
    }
}