using BaseLib.Abstracts;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowRegent;

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
        CardModel[] cards = [
            ModelDb.Card<CelestialMight>(),
            ModelDb.Card<CollisionCourse>(),
            ModelDb.Card<KnowThyPlace>(),
            ModelDb.Card<Patter>(),
            ModelDb.Card<LunarBlast>(),
            ModelDb.Card<KinglyPunch>(),
            ModelDb.Card<KinglyKick>(),
            ModelDb.Card<Terraforming>(),
            ModelDb.Card<Prophesize>(),
            ModelDb.Card<Supermassive>(),
            ModelDb.Card<PillarOfCreation>(),
            ModelDb.Card<HeavenlyDrill>(),
            ModelDb.Card<MakeItSo>(),
            ModelDb.Card<CrashLanding>(),
            ModelDb.Card<Arsenal>(),
            //"Almost identical"
            //Solar Strike
            //Glow
            //Gather Light
        ];

        var sts2Assembly = typeof(ModelDb).Assembly;
        CardModel[] extraCards = ModelDbCompatibility.GetCardModelsSafely([
            sts2Assembly.GetType("MegaCrit.Sts2.Core.Models.Cards.Constellation"),
            sts2Assembly.GetType("MegaCrit.Sts2.Core.Models.Cards.Plot")
        ]);

        return cards.Concat(extraCards).ToArray<CardModel>();
    }
}
