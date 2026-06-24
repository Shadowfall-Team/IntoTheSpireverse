using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character;

public class ShadowNecrobinderCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_necrobinder";
    public override string EnergyColorName => "necrobinder"; // may need to be copied to fix relics?

    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0.965f; //Hue; changes the color.
    public override float S => 0.55f; //Saturation
    public override float V => 1.2f; //Brightness

    public override string CardFrameMaterialPath => "shadow_necrobinder";

    //Color of small card icons
    public override Color DeckEntryCardColor => new("6B4658");
    public override Color EnergyOutlineColor => new("702D6F");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
			ModelDb.Card<CallOfTheVoid>(),
			ModelDb.Card<DanseMacabre>(),
			ModelDb.Card<DeathMarch>(),
			ModelDb.Card<Debilitate>(),
			ModelDb.Card<Defile>(),
			ModelDb.Card<Defy>(),
			ModelDb.Card<Delay>(),
			ModelDb.Card<Demesne>(),
			ModelDb.Card<DrainPower>(),
			ModelDb.Card<Dredge>(),
			ModelDb.Card<EnfeeblingTouch>(),
			ModelDb.Card<Fear>(),
			ModelDb.Card<ForbiddenGrimoire>(), // Replace with Necronomicon when CustomReward is done
			ModelDb.Card<Friendship>(),
			ModelDb.Card<Graveblast>(),
			ModelDb.Card<Lethality>(),
			ModelDb.Card<PullFromBelow>(),
			ModelDb.Card<Putrefy>(),
			ModelDb.Card<SculptingStrike>(),
			ModelDb.Card<SpiritOfAsh>(),
			ModelDb.Card<Transfigure>(),
			ModelDb.Card<Undeath>(),
			ModelDb.Card<Wisp>()
        ];
    }
}
