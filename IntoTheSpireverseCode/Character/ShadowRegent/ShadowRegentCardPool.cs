using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent;

public class ShadowRegentCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_regent";
    public override string EnergyColorName => "regent";

    public override string CardFrameMaterialPath => "shadow_regent";
    public override Color DeckEntryCardColor => new("E36600");
    public override Color EnergyOutlineColor => new("803D0E");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
        [
            ModelDb.Card<CelestialMight>(),
            ModelDb.Card<CollisionCourse>(),
            ModelDb.Card<KnowThyPlace>(),
            ModelDb.Card<LunarBlast>(),
            ModelDb.Card<Glimmer>(),
            ModelDb.Card<PillarOfCreation>(),
            ModelDb.Card<HeavenlyDrill>(),
            ModelDb.Card<MakeItSo>(),
            ModelDb.Card<CrashLanding>(),
            ModelDb.Card<Arsenal>(),
            ModelDb.Card<Royalties>(),
            ModelDb.Card<Orbit>(),

            //Close enough to be considered a duplicate: 
            //Solar Strike
            //Glow
            //Gather Light

            // Multiplayer cards
            ModelDb.Card<Tutor>(),
            ModelDb.Card<Plot>(),
        ];
    }
}
