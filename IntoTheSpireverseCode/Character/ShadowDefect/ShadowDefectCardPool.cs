using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowDefect;

public class ShadowDefectCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_defect";
    public override string EnergyColorName => "defect";

    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0.55f; //Hue; changes the color.
    public override float S => 0.9f; //Saturation
    public override float V => 1f; //Brightness

    public override string CardFrameMaterialPath => "shadow_defect";

    //Color of small card icons
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
