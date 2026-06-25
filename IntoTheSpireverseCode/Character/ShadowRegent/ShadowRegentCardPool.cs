using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;
using Supermassive = IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent.Supermassive;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class ShadowRegentCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_regent";
    public override string EnergyColorName => "regent";
    public override Color DeckEntryCardColor => new("E36600");
    public override string CardFrameMaterialPath => "shadow_regent";
    public override Color EnergyOutlineColor => new("803D0E");
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
            ModelDb.Card<StrikeRegent>(),
            ModelDb.Card<DefendRegent>(),
            ModelDb.Card<Load>(),
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
            ModelDb.Card<GarbageDay>(),
            ModelDb.Card<TrialOfKnowledge>(),
            ModelDb.Card<TrialOfWeaponry>(),
            ModelDb.Card<Cards.ShadowRegent.Convergence>(),
            ModelDb.Card<Contraband>(),
            ModelDb.Card<SpacePirate>(),
            ModelDb.Card<MirrorImage>(),
            ModelDb.Card<TargetAcquired>(),
            ModelDb.Card<Siege>(),
            ModelDb.Card<Terraforming>(),
            ModelDb.Card<DefensiveCannonade>(),
            ModelDb.Card<Prophesize>(),
            ModelDb.Card<GetStronger>(),
            ModelDb.Card<Hoard>(),
            ModelDb.Card<Armada>(),
            ModelDb.Card<TradeRoutes>(),
            ModelDb.Card<CrystalChassis>(),
            ModelDb.Card<PillarOfCreation>(),
            ModelDb.Card<Orbit>(),
            ModelDb.Card<SpectrumShift>(),
            ModelDb.Card<TheLostBlade>(),
            ModelDb.Card<HeavenlyDrill>(),
            ModelDb.Card<MakeItSo>(),
            // ModelDb.Card<AstralStrike>(),
            ModelDb.Card<CrashLanding>(),
            ModelDb.Card<EMPulse>(),
            ModelDb.Card<Smuggle>(),
            ModelDb.Card<TheStarsAlign>(),
            ModelDb.Card<FireEverything>(),
            ModelDb.Card<PoweredThrusters>(),
            ModelDb.Card<Successor>(),
            ModelDb.Card<Misdirection>(),
            ModelDb.Card<PoweredBarrier>(),
            ModelDb.Card<RedGiant>(),
            ModelDb.Card<ShipMaintenance>(),
            ModelDb.Card<EscortMe>(),
            ModelDb.Card<Arsenal>(),
            ModelDb.Card<SpeedHarvest>(),
            ModelDb.Card<TrialOfSpace>(),
            ModelDb.Card<BigGuns>(),
            ModelDb.Card<TrialOfOne>(),
            ModelDb.Card<Construct>(),
            ModelDb.Card<AssemblyRequired>(),
            ModelDb.Card<Grapeshot>(),
            ModelDb.Card<MonarchsGaze>(),
            ModelDb.Card<ExaltedForm>(),
            ModelDb.Card<Constellation>(),
            ModelDb.Card<Cards.ShadowRegent.TheSealedThrone>(),
        ];
    }
}
