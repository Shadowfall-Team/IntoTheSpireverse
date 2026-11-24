using BaseLib.Abstracts;
using Godot;
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

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return
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
            ModelDb.Card<CalculatedGamble>(),

            // Multiplayer Cards
            ModelDb.Card<Fade>(),
            ModelDb.Card<Concoct>(),
        ];
    }
}
