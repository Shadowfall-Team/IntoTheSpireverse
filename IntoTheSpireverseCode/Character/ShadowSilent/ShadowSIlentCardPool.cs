using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowSilent;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class ShadowSilentCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_silent";
    public override string EnergyColorName => "silent";


	public override string CardFrameMaterialPath => "shadow_silent";

	public override Color DeckEntryCardColor => new("5EBD00");

	public override Color EnergyOutlineColor => new("1A6625");

    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 1f; //Hue; changes the color.
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness
    
    public override bool IsColorless => false;
    
    protected override CardModel[] GenerateAllCards()
    {
        return
        [
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
            ModelDb.Card<LegSweep>(),
            ModelDb.Card<Malaise>(),
            ModelDb.Card<Nightmare>(),
            ModelDb.Card<Predator>(),
            ModelDb.Card<Skewer>(),
            ModelDb.Card<StormOfSteel>(),
            ModelDb.Card<TheHunt>(),
            ModelDb.Card<Tracking>()
        ];
    }
}
