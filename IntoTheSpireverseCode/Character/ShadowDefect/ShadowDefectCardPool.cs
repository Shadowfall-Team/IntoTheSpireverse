using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect;

public class ShadowDefectCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_defect";
    public override string EnergyColorName => "defect";

    public override string CardFrameMaterialPath => "shadow_defect";
    public override Color DeckEntryCardColor => new("3EB3ED");
    public override Color EnergyOutlineColor => new("1D5673");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return new CardModel[]
        {
            ModelDb.Card<StrikeDefect>(),
            ModelDb.Card<DefendDefect>(),
            ModelDb.Card<BeamCell>(),
            ModelDb.Card<Claw>(),
            ModelDb.Card<Barrage>(),
            ModelDb.Card<ColdSnap>(),
            ModelDb.Card<SweepingBeam>(),
            ModelDb.Card<Turbo>(),
            ModelDb.Card<ChargeBattery>(),
            ModelDb.Card<Hologram>(),
            ModelDb.Card<Scrape>(),
            ModelDb.Card<Null>(),
            ModelDb.Card<RocketPunch>(),
            ModelDb.Card<Darkness>(),
            ModelDb.Card<EnergySurge>(),
            ModelDb.Card<WhiteNoise>(),
            ModelDb.Card<Glacier>(),
            ModelDb.Card<ShadowShield>(),
            ModelDb.Card<Iteration>(),
            ModelDb.Card<Loop>(),
            ModelDb.Card<BulkUp>(),
            ModelDb.Card<Feral>(),
            ModelDb.Card<Shatter>(),
            ModelDb.Card<AllForOne>(),
            ModelDb.Card<Ignition>(),
            ModelDb.Card<SignalBoost>(),
            ModelDb.Card<Defragment>(),
            ModelDb.Card<MachineLearning>(),
            ModelDb.Card<MegaCrit.Sts2.Core.Models.Cards.Buffer>(),
            ModelDb.Card<ConsumingShadow>(),
        };
    }
}
