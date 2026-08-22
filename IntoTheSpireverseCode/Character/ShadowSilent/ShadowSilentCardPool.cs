using BaseLib.Abstracts;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowSilent;

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
        CardModel[] cards =
        [
            ModelDb.Card<BulletTime>(),
            ModelDb.Card<Burst>(),
            ModelDb.Card<DaggerThrow>(),
            ModelDb.Card<Expose>(),
            ModelDb.Card<LegSweep>(),
            ModelDb.Card<Malaise>(),
            ModelDb.Card<PoisonedStab>(),
            ModelDb.Card<Predator>(),
            ModelDb.Card<SerpentForm>(),
            ModelDb.Card<Snakebite>(),
            ModelDb.Card<ToolsOfTheTrade>(),
            ModelDb.Card<Haze>(),
        ];

        var sts2Assembly = typeof(ModelDb).Assembly;
        var extraCards = ModelDbCompatibility.GetCardModelsSafely([
            sts2Assembly.GetType("MegaCrit.Sts2.Core.Models.Cards.Fade"),
            sts2Assembly.GetType("MegaCrit.Sts2.Core.Models.Cards.Concoct")
        ]);

        cards = [.. cards, .. extraCards];

        return cards;
    }
}
