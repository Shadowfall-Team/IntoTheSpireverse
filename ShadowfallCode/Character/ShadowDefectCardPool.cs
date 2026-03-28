using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowDefectCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_defect";
    public override string EnergyColorName => "defect";

    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 1f; //Hue; changes the color.
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness

    //Alternatively, leave these values at 1 and provide a custom frame image.
    /*public override Texture2D CustomFrame(CustomCardModel card)
    {
        //This will attempt to load Shadowfall/images/cards/frame.png
        return PreloadManager.Cache.GetTexture2D("cards/frame.png".ImagePath());
    }*/

    //Color of small card icons
    public override Color DeckEntryCardColor => new("ffffff");

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
