using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Shadowfall.ShadowfallCode.Character.ShadowDefect;

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
            (CardModel) ModelDb.Card<StrikeDefect>(),
            (CardModel) ModelDb.Card<DefendDefect>(),
            (CardModel) ModelDb.Card<BeamCell>(),
            (CardModel) ModelDb.Card<Claw>(),
            (CardModel) ModelDb.Card<Barrage>(),
            (CardModel) ModelDb.Card<ColdSnap>(),
            (CardModel) ModelDb.Card<SweepingBeam>(),
            (CardModel) ModelDb.Card<Turbo>(),
            (CardModel) ModelDb.Card<ChargeBattery>(),
            (CardModel) ModelDb.Card<Hologram>(),
            (CardModel) ModelDb.Card<Scrape>(),
            (CardModel) ModelDb.Card<Null>(),
            (CardModel) ModelDb.Card<RocketPunch>(),
            (CardModel) ModelDb.Card<Darkness>(),
            (CardModel) ModelDb.Card<EnergySurge>(),
            (CardModel) ModelDb.Card<WhiteNoise>(),
            (CardModel) ModelDb.Card<Glacier>(),
            (CardModel) ModelDb.Card<ShadowShield>(),
            (CardModel) ModelDb.Card<Iteration>(),
            (CardModel) ModelDb.Card<Loop>(),
            (CardModel) ModelDb.Card<BulkUp>(),
            (CardModel) ModelDb.Card<Feral>(),
            (CardModel) ModelDb.Card<Shatter>(),
            (CardModel) ModelDb.Card<AllForOne>(),
            (CardModel) ModelDb.Card<Ignition>(),
            (CardModel) ModelDb.Card<SignalBoost>(),
            (CardModel) ModelDb.Card<Defragment>(),
            (CardModel) ModelDb.Card<MachineLearning>(),
            (CardModel) ModelDb.Card<MegaCrit.Sts2.Core.Models.Cards.Buffer>(),
            (CardModel) ModelDb.Card<ConsumingShadow>(),
        };
    }
}
