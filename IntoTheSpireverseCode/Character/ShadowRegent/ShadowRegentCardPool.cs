using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using IntoTheSpireverse.IntoTheSpireverseCode.Cards.ShadowRegent;

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
            ModelDb.Card<CollisionCourse>(),
            ModelDb.Card<KnowThyPlace>(),
            ModelDb.Card<Patter>(),
            ModelDb.Card<Glitterstream>(),
            ModelDb.Card<LunarBlast>(),
            ModelDb.Card<KinglyPunch>(),
            ModelDb.Card<KinglyKick>(),
            ModelDb.Card<Monologue>(),
            ModelDb.Card<Terraforming>(),
            ModelDb.Card<Prophesize>(),
            ModelDb.Card<PillarOfCreation>(),
            ModelDb.Card<Orbit>(),
            ModelDb.Card<SpectrumShift>(),
            ModelDb.Card<HeavenlyDrill>(),
            ModelDb.Card<MakeItSo>(),
            ModelDb.Card<CrashLanding>(),
            ModelDb.Card<Arsenal>(),
            ModelDb.Card<MonarchsGaze>(),
            ModelDb.Card<ExaltedForm>(),
        ];
    }
}
