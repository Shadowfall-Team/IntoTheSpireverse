using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Shadowfall.ShadowfallCode.Cards.ShadowSilent;
using Adrenaline = Shadowfall.ShadowfallCode.Cards.ShadowSilent.Adrenaline;
using DeadlyPoison = Shadowfall.ShadowfallCode.Cards.ShadowSilent.DeadlyPoison;
using Accelerant = Shadowfall.ShadowfallCode.Cards.ShadowSilent.Accelerant;

namespace Shadowfall.ShadowfallCode.Character;

public class ShadowSilentCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_silent";
    public override string EnergyColorName => "silent";

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
        return
        [
            ModelDb.Card<Accelerant>(),
            ModelDb.Card<Adrenaline>(),
            ModelDb.Card<DeadlyPoison>(),
            ModelDb.Card<MassAppeal>(),
            ModelDb.Card<Accuracy>(),
            ModelDb.Card<Acrobatics>(),
            ModelDb.Card<Afterimage>(),
            ModelDb.Card<Backflip>(),
            ModelDb.Card<BladeDance>(),
            ModelDb.Card<Blur>(),
            ModelDb.Card<Burst>(),
            ModelDb.Card<CloakAndDagger>(),
            ModelDb.Card<DaggerThrow>(),
            ModelDb.Card<DodgeAndRoll>(),
            ModelDb.Card<EchoingSlash>(),
            ModelDb.Card<EscapePlan>(),
            ModelDb.Card<Expose>(),
            ModelDb.Card<FanOfKnives>(),
            ModelDb.Card<Flanking>(),
            ModelDb.Card<Footwork>(),
            ModelDb.Card<HiddenDaggers>(),
            ModelDb.Card<InfiniteBlades>(),
            ModelDb.Card<LegSweep>(),
            ModelDb.Card<Malaise>(),
            ModelDb.Card<MementoMori>(),
            ModelDb.Card<Nightmare>(),
            ModelDb.Card<PiercingWail>(),
            ModelDb.Card<Predator>(),
            ModelDb.Card<Skewer>(),
            ModelDb.Card<StormOfSteel>(),
            ModelDb.Card<TheHunt>(),
            ModelDb.Card<ToolsOfTheTrade>(),
            ModelDb.Card<Tracking>()
        ];
    }
}