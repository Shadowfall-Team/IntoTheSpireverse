using BaseLib.Abstracts;
using Godot;
using IntoTheSpireverse.IntoTheSpireverseCode.Compatibility;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Character.ShadowIronclad;

public class ShadowIroncladCardPool : CustomCardPoolModel
{
    public override string Title => "shadow_ironclad";
    public override string EnergyColorName => "ironclad";

    public override float H => 1f;
    public override float S => 1f;
    public override float V => 1f;

    public override string CardFrameMaterialPath => "shadow_ironclad";
    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        CardModel[] cards = [
            ModelDb.Card<BodySlam>(),
            ModelDb.Card<Breakthrough>(),
            ModelDb.Card<Headbutt>(),
            ModelDb.Card<Bloodletting>(),
            ModelDb.Card<BloodWall>(),
            ModelDb.Card<Spite>(),
            ModelDb.Card<Hemokinesis>(),
            ModelDb.Card<Bludgeon>(),
            ModelDb.Card<DemonicShield>(),
            ModelDb.Card<Taunt>(),
            ModelDb.Card<Inferno>(),
            ModelDb.Card<StoneArmor>(),
            ModelDb.Card<Cascade>(),
            ModelDb.Card<NotYet>(),
            ModelDb.Card<CrimsonMantle>(),
            ModelDb.Card<PrimalForce>(),
            ModelDb.Card<Aggression>(),
            ModelDb.Card<Juggernaut>(),
        ];

        var sts2Assembly = typeof(ModelDb).Assembly;
        CardModel[] extraCards = ModelDbCompatibility.GetCardModelsSafely([
            sts2Assembly.GetType("MegaCrit.Sts2.Core.Models.Cards.Midnight"),
            sts2Assembly.GetType("MegaCrit.Sts2.Core.Models.Cards.Blaze"),
            sts2Assembly.GetType("MegaCrit.Sts2.Core.Models.Cards.Outrage")
        ]);

        return cards.Concat(extraCards).ToArray<CardModel>();
    }
}
